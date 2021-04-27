using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.PluginCore;
using HBK.Storage.Utils;
using HBK.Storage.VideoSubTitleCombinePlugin.CombineHandler;
using HBK.Storage.VideoSubTitleCombinePlugin.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSubTitleCombinePlugin
{
    public class VideoSubTitleCombineTaskManager : PluginTaskManagerBase<VideoSubTitleCombineTaskManager, VideoSubTitleCombineTaskOptions>
    {

        public VideoSubTitleCombineTaskManager(ILogger<VideoSubTitleCombineTaskManager> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        protected override bool ExecuteInternal(PluginTaskModel taskModel)
        {
            using (var scope = base._serviceProvider.CreateScope())
            {
                if (taskModel.FileEntity.ExtendProperty == null)
                {
                    return false;
                }
                var extendProperty = JsonConvert.DeserializeObject<VideoSubTitleCombineExtendProperty>(taskModel.FileEntity.ExtendProperty);
                if (extendProperty == null || extendProperty.SubTitleTrackInfos == null)
                {
                    return false;
                }
                base._logger.LogInformation("[{0}] 開始合併檔案 ID {1} 的檔案 {2} 之影片和字幕檔案", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);
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

                var handler = scope.ServiceProvider.GetRequiredService<CombineHandlerBase>();
                foreach (var subTitleTrack in extendProperty.SubTitleTrackInfos)
                {
                    base._logger.LogInformation("[{0}] 開始合併檔案 ID {1} 的檔案 {2} 的 {3} 字幕軌", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name, subTitleTrack.TrackName);

                    var subTitleFileEntityStorage = storageProviderService.GetFileEntityStorageAsync(taskModel.StorageProviderId, null, subTitleTrack.SubTitleFileEntityId).Result;
                    IAsyncFileInfo subTitleFileInfo = fileEntityStorageService.TryFetchFileInfoAsync(subTitleFileEntityStorage.FileEntityStorageId).Result;
                    if (subTitleFileInfo == null)
                    {
                        continue;
                    }

                    string subTitleFile = Path.Combine(VideoSubTitleCombineTaskManager.CurrentDirectory, Guid.NewGuid().ToString() + Path.GetExtension(subTitleFileEntityStorage.FileEntity.Name));
                    using (var fstream = File.Create(subTitleFile))
                    {
                        subTitleFileInfo.CreateReadStream().CopyTo(fstream);
                    }

                    string outputVideoFile = Path.Combine(workingDirectory, Guid.NewGuid().ToString() + Path.GetExtension(taskModel.FileEntity.Name));
                    var result = handler.Execute(new CombineHandlerTaskModel()
                    {
                        FFmpegLocation = base.Options.FFmpegLocation,
                        Identity = taskId.ToString(),
                        OutputFileName = outputVideoFile,
                        SubTitleFileName = Path.GetFileName(subTitleFile),
                        VideoFileName = sourceVideoFile
                    });

                    if (result.Success)
                    {
                        using (FileStream fs = File.OpenRead(outputVideoFile))
                        {
                            var tags = new List<FileEntityTag>();
                            tags.AddRange(subTitleTrack.CompleteTags.Select(x => new FileEntityTag() { Value = x }));
                            tags.Add(new FileEntityTag() { Value = base.Options.Identity });
                            tags.Add(new FileEntityTag() { Value = subTitleTrack.TrackName });

                            _ = storageProviderService
                                .UploadFileEntityAsync(taskModel.StorageProviderId,
                                    null,
                                    new FileEntity()
                                    {
                                        MimeType = taskModel.FileEntity.MimeType,
                                        Name = Path.GetFileNameWithoutExtension(taskModel.FileEntity.Name) + "-Combine" + Path.GetExtension(taskModel.FileEntity.Name),
                                        Size = fs.Length,
                                        Status = FileEntityStatusEnum.None,
                                        FileEntityTag = tags,
                                        ParentFileEntityID = taskModel.FileEntity.FileEntityId
                                    },
                                    fs, this.Options.Identity).Result;
                        }
                        FileOperator.DeleteSaftey(outputVideoFile);
                    }
                    else
                    {
                        fileEntityService.AppendTagAsync(taskModel.FileEntity.FileEntityId, $"Combine { subTitleTrack.TrackName } Failed");
                    }

                    FileOperator.DeleteSaftey(subTitleFile);
                    base._logger.LogInformation("[{0}] 檔案 ID {1} 的檔案 {2} 的 {3} 字幕軌 合併完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name, subTitleTrack.TrackName);
                }

                DirectoryOperator.DeleteSaftey(workingDirectory, true);
            }
            base._logger.LogInformation("[{0}] 合併檔案 ID {1} 的檔案 {2} 之影片和字幕檔案完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);
            return true;
        }

        public static string CurrentDirectory
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
        }
    }
}
