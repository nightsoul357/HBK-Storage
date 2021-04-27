﻿using HBK.Storage.Adapter.Enums;
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
                base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 屬於例外格式，略過其 Metadata 的處理", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);
                return false;
            }

            using (var scope = base._serviceProvider.CreateScope())
            {
                base._logger.LogInformation("[{0}] 開始處理檔案 ID {1} 的 {2} 之 Metadata", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);
                var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                var fileEntityStorage = storageProviderService.GetFileEntityStorageAsync(taskModel.StorageProviderId, null, taskModel.FileEntity.FileEntityId).Result;
                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return false;
                }

                Guid taskId = Guid.NewGuid();
                string workingDirectory = Path.Combine(base.Options.WorkingDirectory, taskId.ToString());
                Directory.CreateDirectory(workingDirectory);

                string videoFileName = Path.Combine(workingDirectory, Guid.NewGuid() + Path.GetExtension(taskModel.FileEntity.Name));
                using (var fs = File.Create(videoFileName))
                {
                    fileInfo.CreateReadStream().CopyTo(fs);
                }

                var videoFile = new MediaFile { Filename = videoFileName };
                using var engine = new Engine(base.Options.FFmpegLocation);
                engine.GetMetadata(videoFile);

                var perSecond = videoFile.Metadata.Duration.TotalSeconds / (base.Options.PreviewsCount + 1);
                List<string> previewsFiles = new List<string>();
                for (int i = 1; i <= base.Options.PreviewsCount; i++)
                {
                    var outputFile = new MediaFile { Filename = Path.Combine(workingDirectory, Guid.NewGuid().ToString() + ".jpg") };
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
                jobject.Add(new JProperty("MediaMetaData", meta));
                taskModel.FileEntity.ExtendProperty = jobject.ToString();
                taskModel.FileEntity = fileEntityService.UpdateAsync(taskModel.FileEntity).Result;

                for (int i = 1; i <= previewsFiles.Count; i++)
                {
                    using (var previewfs = File.OpenRead(previewsFiles[i - 1]))
                    {
                        _ = storageProviderService
                            .UploadFileEntityAsync(taskModel.StorageProviderId,
                                null,
                                new FileEntity()
                                {
                                    MimeType = "image/jpg",
                                    Name = $"preview-{i}",
                                    Size = previewfs.Length,
                                    Status = FileEntityStatusEnum.None,
                                    FileEntityTag = new List<FileEntityTag>()
                                    {
                                    new FileEntityTag()
                                    {
                                        Value = $"preview-{i}"
                                    }
                                    },
                                    ParentFileEntityID = taskModel.FileEntity.FileEntityId
                                },
                                previewfs, this.Options.Identity).Result;
                    }
                }

                DirectoryOperator.DeleteSaftey(workingDirectory, true);
                base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 之 Metadata 處理完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);
                return true;
            }
        }
    }
}
