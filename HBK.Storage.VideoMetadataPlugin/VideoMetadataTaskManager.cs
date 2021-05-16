using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.PluginCore;
using HBK.Storage.Utils;
using HBK.Storage.VideoMetadataPlugin.Models;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoMetadataPlugin
{
    public class VideoMetadataTaskManager : PluginTaskManagerBase<VideoMetadataTaskManager, VideoMetadataTaskManagerOptions>
    {
        public VideoMetadataTaskManager(ILogger<VideoMetadataTaskManager> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        protected override bool ExecuteInternal(PluginTaskModel taskModel)
        {
            if (base.Options.ExceptionMimeTypes.Any(x => x == taskModel.FileEntity.MimeType))
            {
                base.LogInformation(taskModel, null, "此檔案屬於例外格式，略過其 Metadata 的處理");
                return false;
            }

            using (var scope = base._serviceProvider.CreateScope())
            {
                var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                var fileEntityStorage = storageProviderService.GetFileEntityStorageAsync(taskModel.StorageProviderId, null, taskModel.FileEntity.FileEntityId).Result;

                base.RemoveResidueFileEntity(fileEntityService, taskModel);

                base.LogInformation(taskModel, null, "任務開始 - 處理 Metadata");

                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return false;
                }

                string workingDirectory = Path.Combine(base.Options.WorkingDirectory, taskModel.TaskId.ToString());
                Directory.CreateDirectory(workingDirectory);

                string videoFileName = Path.Combine(workingDirectory, Guid.NewGuid() + Path.GetExtension(taskModel.FileEntity.Name));
                base.DownloadFileEntity(taskModel.TaskId, fileInfo, taskModel.FileEntity, videoFileName);

                var videoFile = new MediaFile { Filename = videoFileName };
                string newFFmpegLocation = Path.Combine(workingDirectory, "ffmpeg.exe");
                File.Copy(base.Options.FFmpegLocation, newFFmpegLocation);

                using var engine = new Engine(newFFmpegLocation);
                engine.GetMetadata(videoFile);

                var perSecond = videoFile.Metadata.Duration.TotalSeconds / (base.Options.PreviewsCount + 1);
                List<string> previewsFiles = new List<string>();
                for (int i = 1; i <= base.Options.PreviewsCount; i++)
                {
                    var outputFile = new MediaFile { Filename = Path.Combine(workingDirectory, Guid.NewGuid().ToString() + ".jpeg") };
                    var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(perSecond * i) };
                    engine.GetThumbnail(videoFile, outputFile, options);
                    previewsFiles.Add(outputFile.Filename);
                }

                JObject jobject;
                if (taskModel.FileEntity.ExtendProperty != null && !String.IsNullOrWhiteSpace(taskModel.FileEntity.ExtendProperty))
                {
                    jobject = JObject.Parse(taskModel.FileEntity.ExtendProperty);
                }
                else
                {
                    jobject = new JObject();
                }
                var meta = JObject.Parse(JsonConvert.SerializeObject(videoFile.Metadata));
                if (jobject.TryGetValue("MediaMetaData", out JToken temp))
                {
                    jobject.Remove("MediaMetaData");
                }
                jobject.Add(new JProperty("MediaMetaData", meta));
                taskModel.FileEntity.ExtendProperty = jobject.ToString();
                taskModel.FileEntity = fileEntityService.UpdateAsync(taskModel.FileEntity).Result;

                List<FileEntity> processingFileEntities = new List<FileEntity>();
                for (int i = 1; i <= previewsFiles.Count; i++)
                {
                    using (var previewfs = File.OpenRead(previewsFiles[i - 1]))
                    {
                        var preFileEntity = storageProviderService
                            .UploadFileEntityAsync(taskModel.StorageProviderId,
                                null,
                                new FileEntity()
                                {
                                    MimeType = "image/jpeg",
                                    Name = $"preview-{i}.jpeg",
                                    Size = previewfs.Length,
                                    Status = FileEntityStatusEnum.Processing,
                                    FileEntityTag = new List<FileEntityTag>()
                                    {
                                        new FileEntityTag()
                                        {
                                            Value = $"preview-{i}"
                                        },
                                        new FileEntityTag()
                                        {
                                            Value = base.Options.Identity
                                        },
                                        new FileEntityTag() // 圖片壓縮
                                        {
                                            Value = "Require-Compress-Image"
                                        }
                                    },
                                    ParentFileEntityID = taskModel.FileEntity.FileEntityId
                                },
                                previewfs, this.Options.Identity).Result;

                        base.LogInformation(taskModel, null, "上傳第 {0} 張預覽圖完成", i);
                        processingFileEntities.Add(preFileEntity);
                    }
                }

                processingFileEntities.ForEach(x => x.Status = x.Status & ~FileEntityStatusEnum.Processing);
                fileEntityService.UpdateBatchAsync(processingFileEntities).Wait();

                DirectoryOperator.DeleteSaftey(workingDirectory, true);
                base.LogInformation(taskModel, null, "任務結束 - 處理 Metadata");
                return true;
            }
        }
    }
}
