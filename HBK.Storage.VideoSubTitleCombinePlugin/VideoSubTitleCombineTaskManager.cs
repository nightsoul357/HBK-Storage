using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.PluginCore;
using HBK.Storage.Utils;
using HBK.Storage.VideoSubTitleCombinePlugin.CombineHandler;
using HBK.Storage.VideoSubTitleCombinePlugin.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        private readonly IHostEnvironment _hostEnvironment;
        public VideoSubTitleCombineTaskManager(ILogger<VideoSubTitleCombineTaskManager> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
            _hostEnvironment = base._serviceScope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        }

        protected override bool ExecuteInternal(PluginTaskModel taskModel)
        {
            using (var scope = base._serviceProvider.CreateScope())
            {
                if (taskModel.FileEntity.ExtendProperty == null)
                {
                    base.LogInformation(taskModel, null, "因為 ExtendProperty 為 NULL 而無法正確執行影片字幕合併任務");
                    return false;
                }
                var extendProperty = JsonConvert.DeserializeObject<VideoSubTitleCombineExtendProperty>(taskModel.FileEntity.ExtendProperty);
                if (extendProperty == null || extendProperty.SubTitleTrackInfos == null)
                {
                    base.LogInformation(taskModel, null, "因為 ExtendProperty 為 NULL 或 SubTitleTrackInfos 而無法正確執行影片字幕合併任務");
                    return false;
                }
                var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                var fileEntityStorage = storageProviderService.GetFileEntityStorageAsync(taskModel.StorageProviderId, null, taskModel.FileEntity.FileEntityId).Result;
                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;

                base.RemoveResidueFileEntity(fileEntityService, taskModel);
                base.LogInformation(taskModel, null, "任務開始 - 合併影片跟字幕");

                string workingDirectory = Path.Combine(base.Options.WorkingDirectory, taskModel.TaskId.ToString());
                string sourceDirecotry = Path.Combine(workingDirectory, "src");
                Directory.CreateDirectory(sourceDirecotry);

                if (fileInfo == null)
                {
                    return false;
                }

                string sourceVideoFile = Path.Combine(sourceDirecotry, Guid.NewGuid().ToString() + Path.GetExtension(taskModel.FileEntity.Name));
                base.DownloadFileEntity(taskModel.TaskId, fileInfo, taskModel.FileEntity, sourceVideoFile);

                string newFFmpegLocation = Path.Combine(workingDirectory, "ffmpeg.exe");
                File.Copy(base.Options.FFmpegLocation, newFFmpegLocation);
                var handler = scope.ServiceProvider.GetRequiredService<CombineHandlerBase>();
                List<FileEntity> processFileEntities = new List<FileEntity>();

                foreach (var subTitleTrack in extendProperty.SubTitleTrackInfos)
                {
                    base.LogInformation(taskModel, null, "開始合併 {0} 字幕軌", subTitleTrack.TrackName);
                    var subTitleFileEntityStorage = storageProviderService.GetFileEntityStorageAsync(taskModel.StorageProviderId, null, subTitleTrack.SubTitleFileEntityId).Result;
                    IAsyncFileInfo subTitleFileInfo = fileEntityStorageService.TryFetchFileInfoAsync(subTitleFileEntityStorage.FileEntityStorageId).Result;
                    if (subTitleFileInfo == null)
                    {
                        continue;
                    }

                    string subTitleFile = Path.Combine(this.CurrentDirectory, Guid.NewGuid().ToString() + Path.GetExtension(subTitleFileEntityStorage.FileEntity.Name));
                    var subTitleFileEntity = fileEntityService.FindByIdAsync(subTitleTrack.SubTitleFileEntityId).Result;
                    base.DownloadFileEntity(taskModel.TaskId, subTitleFileInfo, subTitleFileEntity, subTitleFile);

                    string outputVideoFile = Path.Combine(workingDirectory, Guid.NewGuid().ToString() + Path.GetExtension(taskModel.FileEntity.Name));
                    var result = handler.Execute(new CombineHandlerTaskModel()
                    {
                        FFmpegLocation = newFFmpegLocation,
                        Identity = base.Options.Identity,
                        OutputFileName = outputVideoFile,
                        SubTitleFileName = Path.GetFileName(subTitleFile),
                        VideoFileName = sourceVideoFile
                    }, (e, arg) =>
                    {
                        base.LogInformation(taskModel, arg, "正在合併 {0} 字幕軌 ...", subTitleTrack.TrackName);
                    }, (e, arg) =>
                    {
                        base.LogInformation(taskModel, arg, "{0} 字幕軌合併完成", subTitleTrack.TrackName);
                    });

                    if (result.Success)
                    {
                        using (FileStream fs = File.OpenRead(outputVideoFile))
                        {
                            var tags = new List<FileEntityTag>();
                            tags.AddRange(subTitleTrack.CompleteTags.Select(x => new FileEntityTag() { Value = x }));
                            tags.Add(new FileEntityTag() { Value = base.Options.Identity });
                            tags.Add(new FileEntityTag() { Value = subTitleTrack.TrackName });

                            base.LogInformation(taskModel, null, "開始上傳 {0} 字幕軌合併後的檔案", subTitleTrack.TrackName);

                            var combineFileEntity = storageProviderService
                                .UploadFileEntityAsync(taskModel.StorageProviderId,
                                    null,
                                    new FileEntity()
                                    {
                                        MimeType = taskModel.FileEntity.MimeType,
                                        Name = Path.GetFileNameWithoutExtension(taskModel.FileEntity.Name) + "-Combine" + Path.GetExtension(taskModel.FileEntity.Name),
                                        Size = fs.Length,
                                        Status = FileEntityStatusEnum.Processing,
                                        FileEntityTag = tags,
                                        ParentFileEntityID = taskModel.FileEntity.FileEntityId
                                    },
                                    fs, this.Options.Identity).Result;

                            base.LogInformation(taskModel, null, "上傳 {0} 字幕軌合併後的檔案 完成", subTitleTrack.TrackName);

                            processFileEntities.Add(combineFileEntity);
                        }
                        FileOperator.DeleteSaftey(outputVideoFile);
                    }
                    else
                    {
                        base.LogInformation(taskModel, null, "字幕軌 {0} 合併失敗", subTitleTrack.TrackName);
                        fileEntityService.AppendTagAsync(taskModel.FileEntity.FileEntityId, $"Combine { subTitleTrack.TrackName } Failed");
                    }
                    FileOperator.DeleteSaftey(subTitleFile);

                    base.LogInformation(taskModel, null, "字幕軌 {0} 合併完成", subTitleTrack.TrackName);
                }

                processFileEntities.Select(x => x.Status = x.Status & ~FileEntityStatusEnum.Processing);
                fileEntityService.UpdateBatchAsync(processFileEntities).Wait();

                DirectoryOperator.DeleteSaftey(workingDirectory, true);
            }

            base.LogInformation(taskModel, null, "任務完成 - 合併影片跟字幕");
            return true;
        }

        public string CurrentDirectory
        {
            get
            {
                return _hostEnvironment.ContentRootPath;
            }
        }
    }
}
