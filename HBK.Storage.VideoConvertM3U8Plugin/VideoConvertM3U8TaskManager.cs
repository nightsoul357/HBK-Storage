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
                base.LogInformation(taskModel, null, "任務開始 - 轉換 M3U8");
                string workingDirectory = Path.Combine(base.Options.WorkingDirectory, taskModel.TaskId.ToString());
                string sourceDirecotry = Path.Combine(workingDirectory, "src");
                Directory.CreateDirectory(sourceDirecotry);

                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return false;
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
                });

                if (!result.Success)
                {
                    return false;
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
                return true;
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
                            ParentFileEntityID = taskModel.FileEntity.FileEntityId
                        },
                        fs, this.Options.Identity).Result;

            processingFileEntities.Add(m3u8File);
            base.LogInformation(taskModel, null, "M3U8 檔案 上傳完成");

            int tsnumber = 1;
            foreach (var file in Directory.GetFiles(outputDirectory, "*.ts"))
            {
                base.LogInformation(taskModel, null, "開始上傳第 {0} 個 TS 檔案", tsnumber);

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

                base.LogInformation(taskModel, null, "第 {0} 個 TS 檔案 上船完成", tsnumber);
                processingFileEntities.Add(fts);
                tsnumber++;
            }

            int vttnumber = 1;
            foreach (var file in Directory.GetFiles(outputDirectory, "*.vtt"))
            {
                base.LogInformation(taskModel, null, "開始上傳第 {0} 個 VTT 檔案", vttnumber);

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

                base.LogInformation(taskModel, null, "第 {0} 個 VTT 檔案上傳完成", vttnumber);

                processingFileEntities.Add(fvtt);
                vttnumber++;
            }

            base.LogInformation(taskModel, null, "M3U8 轉換完成所有檔案上傳完成");

            processingFileEntities.ForEach(x => x.Status = x.Status & ~FileEntityStatusEnum.Processing); // 移除處理中
            fileEntityService.UpdateBatchAsync(processingFileEntities).Wait();

        }
    }
}
