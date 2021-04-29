using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.PluginCore;
using HBK.Storage.Utils;
using HBK.Storage.VideoConvertM3U8Plugin.ConvertHandler;
using HBK.Storage.VideoConvertM3U8Plugin.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoConvertM3U8Plugin
{
    public class VideoConvertM3U8TaskManager : PluginTaskManagerBase<VideoConvertM3U8TaskManager, VideoConvertM3U8TaskManagerOptions>
    {
        public VideoConvertM3U8TaskManager(ILogger<VideoConvertM3U8TaskManager> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        protected override bool ExecuteInternal(PluginTaskModel taskModel)
        {
            using (var scope = base._serviceProvider.CreateScope())
            {
                var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                var fileEntityStorage = storageProviderService.GetFileEntityStorageAsync(taskModel.StorageProviderId, null, taskModel.FileEntity.FileEntityId).Result;

                base.RemoveResidueFileEntity(fileEntityService, taskModel);

                base._logger.LogInformation("[{0}] 開始將檔案 ID {1} 的 {2} 轉換為 M3U8 格式", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);
                Guid taskId = Guid.NewGuid();
                string workingDirectory = Path.Combine(base.Options.WorkingDirectory, taskId.ToString());
                string sourceDirecotry = Path.Combine(workingDirectory, "src");
                Directory.CreateDirectory(sourceDirecotry);

                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return false;
                }

                string sourceVideoFile = Path.Combine(sourceDirecotry, Guid.NewGuid().ToString() + Path.GetExtension(taskModel.FileEntity.Name));
                base.DownloadFileEntity(fileInfo, taskModel.FileEntity, sourceVideoFile);

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
                });

                if (!result.Success)
                {
                    return false;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"[{base.Options.Identity}] 檔案 ID {taskModel.FileEntity.FileEntityId} 的 {taskModel.FileEntity.Name} 轉換完成之");
                sb.AppendLine($"M3U8 檔案大小為 : { outputVideo.Length } Bytes");
                sb.AppendLine($"TS 檔案數量為 : { Directory.GetFiles(outputDirectory, "*.ts").Count() } 個");
                sb.AppendLine($"VTT 檔案數量為 : { Directory.GetFiles(outputDirectory, "*.vtt").Count() } 個");
                base._logger.LogInformation(sb.ToString());

                this.UploadOutputDirectory(outputDirectory, outputVideo, taskModel, storageProviderService, fileEntityService);

                DirectoryOperator.DeleteSaftey(workingDirectory, true);

                base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 轉換為 M3U8 格式 轉換成功", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);
                return true;
            }
        }

        private void UploadOutputDirectory(string outputDirectory, string outputVideo, PluginTaskModel taskModel, StorageProviderService storageProviderService, FileEntityService fileEntityService)
        {
            using var fs = File.OpenRead(outputVideo);
            List<FileEntity> processingFileEntities = new List<FileEntity>();

            base._logger.LogInformation("[{0}] 開始上傳 檔案 ID {1} 的 {2} 之 M3U8 檔案", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);
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
                            ParentFileEntityID = taskModel.FileEntity.FileEntityId
                        },
                        fs, this.Options.Identity).Result;

            processingFileEntities.Add(m3u8File);
            base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 之 M3U8 檔案 上傳完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);

            int tsnumber = 1;
            foreach (var file in Directory.GetFiles(outputDirectory, "*.ts"))
            {
                base._logger.LogInformation("[{0}] 開始上傳 檔案 ID {1} 的 {2} 第 {3} 個 TS 檔案", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name, tsnumber);

                using var fsTs = File.OpenRead(file);
                var fts = storageProviderService
                        .UploadFileEntityAsync(taskModel.StorageProviderId,
                            null,
                            new FileEntity()
                            {
                                MimeType = "video/MP2T",
                                Name = Path.GetFileName(file),
                                Size = fsTs.Length,
                                Status = FileEntityStatusEnum.Processing,
                                FileEntityTag = new List<FileEntityTag>()
                                {
                                    new FileEntityTag()
                                    {
                                        Value = $"ts-{tsnumber}"
                                    },
                                    new FileEntityTag()
                                    {
                                        Value = this.Options.Identity
                                    }
                                },
                                ParentFileEntityID = m3u8File.FileEntityId
                            },
                            fsTs, this.Options.Identity).Result;

                base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 第 {3} 個 TS 檔案 上傳完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name, tsnumber);
                processingFileEntities.Add(fts);
                tsnumber++;
            }

            int vttnumber = 1;
            foreach (var file in Directory.GetFiles(outputDirectory, "*.vtt"))
            {
                base._logger.LogInformation("[{0}] 開始上傳 檔案 ID {1} 的 {2} 第 {3} 個 VTT 檔案", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name, vttnumber);

                using var fsVtt = File.OpenRead(file);
                var fvtt = storageProviderService
                        .UploadFileEntityAsync(taskModel.StorageProviderId,
                            null,
                            new FileEntity()
                            {
                                MimeType = "text/vtt",
                                Name = Path.GetFileName(file),
                                Size = fsVtt.Length,
                                Status = FileEntityStatusEnum.Processing,
                                FileEntityTag = new List<FileEntityTag>()
                                {
                                    new FileEntityTag()
                                    {
                                        Value = $"vtt-{vttnumber}"
                                    },
                                    new FileEntityTag()
                                    {
                                        Value = this.Options.Identity
                                    }
                                },
                                ParentFileEntityID = m3u8File.FileEntityId
                            },
                            fsVtt, this.Options.Identity).Result;

                base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 第 {3} 個 VTT 檔案 上傳完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name, vttnumber);

                processingFileEntities.Add(fvtt);
                vttnumber++;
            }

            base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 之 M3U8 轉換完成所有檔案上傳完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);

            processingFileEntities.ForEach(x => x.Status = x.Status & ~FileEntityStatusEnum.Processing); // 移除處理中
            fileEntityService.UpdateBatchAsync(processingFileEntities).Wait();

        }
    }
}
