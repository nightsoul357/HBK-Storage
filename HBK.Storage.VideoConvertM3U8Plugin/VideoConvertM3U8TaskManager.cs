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
                Guid taskId = Guid.NewGuid();
                string workingDirectory = Path.Combine(base.Options.WorkingDirectory, taskId.ToString());
                string sourceDirecotry = Path.Combine(workingDirectory, "src");
                Directory.CreateDirectory(sourceDirecotry);

                var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                var fileEntityStorage = storageProviderService.GetFileEntityStorageAsync(taskModel.StorageProviderId, null, taskModel.FileEntity.FileEntityId).Result;
                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return false;
                }

                var sourceStream = fileInfo.CreateReadStream();
                string sourceVideoFile = Path.Combine(sourceDirecotry, Guid.NewGuid().ToString() + Path.GetExtension(taskModel.FileEntity.Name));
                using (var fstream = File.Create(sourceVideoFile))
                {
                    sourceStream.CopyTo(fstream);
                }

                string outputDirectory = Path.Combine(workingDirectory, "out");
                string outputVideo = Path.Combine(outputDirectory, Guid.NewGuid().ToString() + ".m3u8");
                Directory.CreateDirectory(outputDirectory);
                var handler = scope.ServiceProvider.GetRequiredService<ConvertHandlerBase>();
                var result = handler.Execute(new ConvertHandlerTaskModel()
                {
                    FFmpegLocation = base.Options.FFmpegLocation,
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

                this.UploadOutputDirectory(outputDirectory, outputVideo, taskModel.StorageProviderId, taskModel.FileEntity.FileEntityId, storageProviderService);

                DirectoryOperator.DeleteSaftey(workingDirectory, true);
                return true;
            }
        }

        private void UploadOutputDirectory(string outputDirectory, string outputVideo, Guid storageProviderId, Guid parentFileEntityId, StorageProviderService storageProviderService)
        {
            using var fs = File.OpenRead(outputVideo);
            var m3u8File = storageProviderService
                    .UploadFileEntityAsync(storageProviderId,
                        null,
                        new FileEntity()
                        {
                            MimeType = "application/x-mpegURL",
                            Name = Path.GetFileName(outputVideo),
                            Size = fs.Length,
                            Status = FileEntityStatusEnum.None,
                            FileEntityTag = new List<FileEntityTag>()
                                {
                                    new FileEntityTag()
                                    {
                                        Value = this.Options.Identity
                                    }
                                },
                            ParentFileEntityID = parentFileEntityId
                        },
                        fs, this.Options.Identity).Result;

            int tsnumber = 1;
            foreach (var file in Directory.GetFiles(outputDirectory, "*.ts"))
            {
                using var fsTs = File.OpenRead(file);
                _ = storageProviderService
                        .UploadFileEntityAsync(storageProviderId,
                            null,
                            new FileEntity()
                            {
                                MimeType = "video/MP2T",
                                Name = Path.GetFileName(file),
                                Size = fsTs.Length,
                                Status = FileEntityStatusEnum.None,
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
                tsnumber++;
            }

            int vttnumber = 1;
            foreach (var file in Directory.GetFiles(outputDirectory, "*.vtt"))
            {
                using var fsVtt = File.OpenRead(file);
                _ = storageProviderService
                        .UploadFileEntityAsync(storageProviderId,
                            null,
                            new FileEntity()
                            {
                                MimeType = "text/vtt",
                                Name = Path.GetFileName(file),
                                Size = fsVtt.Length,
                                Status = FileEntityStatusEnum.None,
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
                vttnumber++;
            }


            
        }
    }
}
