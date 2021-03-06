using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.PluginCore;
using HBK.Storage.Utils;
using HBK.Storage.VideoConvertM3U8Plugin.ConvertHandler;
using HBK.Storage.VideoConvertM3U8Plugin.Enums;
using HBK.Storage.VideoConvertM3U8Plugin.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoConvertM3U8Plugin
{
    public class VideoConvertM3U8TaskManager : PluginTaskManagerBase<VideoConvertM3U8TaskManager, VideoConvertM3U8TaskManagerOptions>
    {
        private static Dictionary<UploadFileTypeEnum, string> mimeTypes = new Dictionary<UploadFileTypeEnum, string>()
        {
            [UploadFileTypeEnum.TS] = "video/MP2T",
            [UploadFileTypeEnum.VTT] = "text/vtt"
        };
        private readonly List<StorageTypeEnum> _storageTypes;
        public VideoConvertM3U8TaskManager(ILogger<VideoConvertM3U8TaskManager> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
            _storageTypes = Enum.GetValues<StorageTypeEnum>().Cast<StorageTypeEnum>().ToList();
            if (!base.Options.IsExecuteOnLocalStorage)
            {
                _storageTypes.Remove(StorageTypeEnum.Local);
            }
        }
        protected override ExecuteResultEnum ExecuteInternal(PluginTaskModel taskModel)
        {
            using (var scope = base._serviceProvider.CreateScope())
            {
                var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                var fileEntityStorage = storageProviderService.GetFileEntityStorageAsync(taskModel.StorageProviderId, null, taskModel.FileEntity.FileEntityId, _storageTypes).Result;

                base.RemoveResidueFileEntity(fileEntityService, taskModel);
                base.LogInformation(taskModel, null, "任務開始 - 轉換 M3U8");
                string workingDirectory = Path.Combine(base.Options.WorkingDirectory, taskModel.TaskId.ToString());
                string sourceDirecotry = Path.Combine(workingDirectory, "src");
                Directory.CreateDirectory(sourceDirecotry);

                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return ExecuteResultEnum.Failed;
                }

                string sourceVideoFile = Path.Combine(sourceDirecotry, Guid.NewGuid().ToString() + Path.GetExtension(taskModel.FileEntity.Name));
                base.DownloadFileEntity(taskModel.TaskId, fileInfo, taskModel.FileEntity, sourceVideoFile);

                string outputDirectory = Path.Combine(workingDirectory, "out");
                string outputVideo = Path.Combine(outputDirectory, Guid.NewGuid().ToString() + ".m3u8");
                Directory.CreateDirectory(outputDirectory);
                string newFFmpegLocation = Path.Combine(workingDirectory, "ffmpeg.exe");
                File.Copy(base.Options.FFmpegLocation, newFFmpegLocation);

                var handler = scope.ServiceProvider.GetRequiredService<ConvertHandlerBase>();
                var result = handler.Execute(new ConvertHandlerTaskModel()
                {
                    FFmpegLocation = newFFmpegLocation,
                    Identity = base.Options.Identity,
                    OutputFileName = outputVideo,
                    VideoFileName = sourceVideoFile,
                    TempDirectory = workingDirectory,
                    TSInterval = base.Options.TSInterval
                }, (e, arg) =>
                 {
                     base.LogInformation(taskModel, arg, "正在轉換 M3U8 ...");
                 }, (e, arg) =>
                 {
                     base.LogInformation(taskModel, arg, "M3U8 轉換完成");
                 });

                if (!result.Success)
                {
                    base.LogInformation(taskModel, null, "M3U8 轉換失敗");
                    return ExecuteResultEnum.Failed;
                }
                else
                {
                    base.LogInformation(taskModel, null, "M3U8 轉換成功");
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"M3U8 轉換完成");
                sb.AppendLine($"M3U8 檔案大小為 : { outputVideo.Length } Bytes");
                sb.AppendLine($"TS 檔案數量為 : { Directory.GetFiles(outputDirectory, "*.ts").Count() } 個");
                sb.AppendLine($"VTT 檔案數量為 : { Directory.GetFiles(outputDirectory, "*.vtt").Count() } 個");

                base.LogInformation(taskModel, null, sb.ToString());

                this.UploadOutputDirectory(outputDirectory, outputVideo, taskModel, storageProviderService, fileEntityService);

                DirectoryOperator.DeleteSaftey(workingDirectory, true);

                base.LogInformation(taskModel, null, "任務結束 - 轉換 M3U8");
                return ExecuteResultEnum.Successful;
            }
        }

        private void UploadOutputDirectory(string outputDirectory, string outputVideo, PluginTaskModel taskModel, StorageProviderService storageProviderService, FileEntityService fileEntityService)
        {
            using var fs = File.OpenRead(outputVideo);
            List<FileEntity> processingFileEntities = new List<FileEntity>();

            base.LogInformation(taskModel, null, "開始上傳 M3U8 檔案");
            var m3u8File = storageProviderService
                    .UploadFileEntityAsync(taskModel.StorageProviderId,
                        null,
                        new FileEntity()
                        {
                            MimeType = "application/x-mpegURL",
                            Name = Path.GetFileName(outputVideo),
                            Size = fs.Length,
                            Status = FileEntityStatusEnum.Processing,
                            FileEntityTag = new List<FileEntityTag>()
                                {
                                    new FileEntityTag()
                                    {
                                        Value = this.Options.Identity
                                    }
                                },
                            ParentFileEntityID = taskModel.FileEntity.FileEntityId,
                            CryptoMode = taskModel.FileEntity.CryptoMode
                        },
                        fs, this.Options.Identity, _storageTypes).Result;

            processingFileEntities.Add(m3u8File);
            base.LogInformation(taskModel, null, "M3U8 檔案 上傳完成");

            ConcurrentQueue<UploadFile> uploadFiles = new ConcurrentQueue<UploadFile>();
            int tsnumber = 1;
            foreach (var file in Directory.GetFiles(outputDirectory, "*.ts"))
            {
                uploadFiles.Enqueue(new UploadFile()
                {
                    Filename = file,
                    M3U8FileEntityId = m3u8File.FileEntityId,
                    Number = tsnumber,
                    Type = UploadFileTypeEnum.TS
                });
                tsnumber++;
            }

            int vttnumber = 1;
            foreach (var file in Directory.GetFiles(outputDirectory, "*.vtt"))
            {
                uploadFiles.Enqueue(new UploadFile()
                {
                    Filename = file,
                    M3U8FileEntityId = m3u8File.FileEntityId,
                    Number = vttnumber,
                    Type = UploadFileTypeEnum.VTT
                });
                vttnumber++;
            }

            List<Task> uploadTasks = new List<Task>();
            for (int i = 0; i < base.Options.UploadTaskCount; i++)
            {
                uploadTasks.Add(Task.Factory.StartNew(() =>
                {
                    UploadFile uploadFile;
                    while (uploadFiles.TryDequeue(out uploadFile))
                    {
                        using (var currentScope = base._serviceProvider.CreateScope())
                        {
                            var storageProviderService = currentScope.ServiceProvider.GetRequiredService<StorageProviderService>();
                            processingFileEntities.Add(this.UploadFile(storageProviderService, taskModel, uploadFile));
                        }
                    }
                }));
            }

            Task.WaitAll(uploadTasks.ToArray());

            base.LogInformation(taskModel, null, "M3U8 轉換完成所有檔案上傳完成");

            processingFileEntities.ForEach(x => x.Status = x.Status & ~FileEntityStatusEnum.Processing); // 移除處理中
            fileEntityService.UpdateBatchAsync(processingFileEntities).Wait();
        }

        private FileEntity UploadFile(StorageProviderService storageProviderService, PluginTaskModel taskModel, UploadFile uploadFile)
        {
            base.LogInformation(taskModel, null, "開始上傳第 {0} 個 {1} 檔案", uploadFile.Number, uploadFile.Type.ToString());
            using (var file = File.OpenRead(uploadFile.Filename))
            {
                var fileEntity = storageProviderService
                       .UploadFileEntityAsync(taskModel.StorageProviderId,
                           null,
                           new FileEntity()
                           {
                               MimeType = mimeTypes[uploadFile.Type],
                               Name = Path.GetFileName(uploadFile.Filename),
                               Size = file.Length,
                               Status = FileEntityStatusEnum.Processing,
                               FileEntityTag = new List<FileEntityTag>()
                               {
                                    new FileEntityTag()
                                    {
                                        Value = $"{uploadFile.Type.ToString().ToLower()}-{uploadFile.Number}"
                                    },
                                    new FileEntityTag()
                                    {
                                        Value = this.Options.Identity
                                    }
                               },
                               ParentFileEntityID = uploadFile.M3U8FileEntityId,
                               CryptoMode = taskModel.FileEntity.CryptoMode
                           },
                           file, this.Options.Identity).Result;
                base.LogInformation(taskModel, null, "第 {0} 個 {1} 檔案 上傳完成", uploadFile.Number, uploadFile.Type.ToString());
                return fileEntity;
            }
        }
    }
}
