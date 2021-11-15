using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.PluginCore;
using HBK.Storage.Utils;
using HBK.Storage.VideoSeekPreviewPlugin.Models;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSeekPreviewPlugin
{
    public class VideoSeekPreviewTaskManager : PluginTaskManagerBase<VideoSeekPreviewTaskManager, VideoSeekPreviewTaskManagerOptions>
    {
        private readonly List<StorageTypeEnum> _storageTypes;
        public VideoSeekPreviewTaskManager(ILogger<VideoSeekPreviewTaskManager> logger, IServiceProvider serviceProvider)
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

                base.LogInformation(taskModel, null, "任務開始 - 產生 Seek Preview");

                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return ExecuteResultEnum.Failed;
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
                ConcurrentQueue<PreviewImage> previewsFiles = new ConcurrentQueue<PreviewImage>();
                for (int i = 0; i < videoFile.Metadata.Duration.TotalSeconds; i += base.Options.PreviewInterval)
                {
                    var outputFile = new MediaFile { Filename = Path.Combine(workingDirectory, Guid.NewGuid().ToString() + ".jpeg") };
                    var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i) };
                    engine.GetThumbnail(videoFile, outputFile, options);
                    if (!File.Exists(outputFile.Filename))
                    {
                        break;
                    }
                    string newFile = Path.Combine(workingDirectory, Guid.NewGuid().ToString() + ".jpeg");
                    this.ResizeImage(outputFile.Filename, newFile, base.Options.PreviewWidth);
                    File.Delete(outputFile.Filename);
                    previewsFiles.Enqueue(new PreviewImage()
                    {
                        Filename = newFile,
                        Second = i
                    });
                }
                List<Task> uploadTasks = new List<Task>();
                List<FileEntity> processFileEntities = new List<FileEntity>();

                for (int i = 0; i < base.Options.UploadTaskCount; i++)
                {
                    uploadTasks.Add(Task.Factory.StartNew(() =>
                    {
                        using (var currnetScope = _serviceProvider.CreateScope())
                        {
                            var storageProviderService = currnetScope.ServiceProvider.GetRequiredService<StorageProviderService>();
                            PreviewImage previewImage;
                            while (previewsFiles.TryDequeue(out previewImage))
                            {
                                processFileEntities.Add(this.UploadPreivewImage(storageProviderService, taskModel, previewImage));
                            }
                        }
                    }));
                }

                Task.WaitAll(uploadTasks.ToArray());

                processFileEntities.ForEach(x => x.Status = x.Status & ~FileEntityStatusEnum.Processing);
                fileEntityService.UpdateBatchAsync(processFileEntities).Wait();

                DirectoryOperator.DeleteSaftey(workingDirectory, true);
                base.LogInformation(taskModel, null, "任務結束 - 產生 Seek Preview");

                return ExecuteResultEnum.Successful;
            }
        }
        private FileEntity UploadPreivewImage(StorageProviderService storageProviderService, PluginTaskModel taskModel, PreviewImage previewImage)
        {
            using (var fstream = File.Open(previewImage.Filename, FileMode.Open))
            {
                var fileEntity = storageProviderService
                    .UploadFileEntityAsync(taskModel.StorageProviderId,
                        null,
                        new FileEntity()
                        {
                            MimeType = "image/jpeg",
                            Name = Path.GetFileNameWithoutExtension(taskModel.FileEntity.Name) + $"-Seek-{previewImage.Second}.jpeg",
                            Size = fstream.Length,
                            Status = FileEntityStatusEnum.Processing,
                            FileEntityTag = new List<FileEntityTag>() {
                                        new FileEntityTag()
                                        {
                                            Value = this.Options.Identity
                                        },
                                        new FileEntityTag()
                                        {
                                            Value = $"Seek-{ previewImage.Second }"
                                        }
                            },
                            ParentFileEntityID = taskModel.FileEntity.FileEntityId,
                            CryptoMode = taskModel.FileEntity.CryptoMode
                        },
                        fstream, this.Options.Identity, _storageTypes).Result;

                base.LogInformation(taskModel, null, "第 {0} 秒 Seek Preview 上傳完成", previewImage.Second);
                return fileEntity;
            }
        }
        private void ResizeImage(string source, string output, int newWidth)
        {
            using (Bitmap bitmap = new Bitmap(source))
            {
                int newHeitght = Convert.ToInt32(((float)newWidth / bitmap.Width) * bitmap.Height);
                using (Bitmap newBitmap = new Bitmap(bitmap, newWidth, newHeitght))
                {
                    var result = this.CompressImage(newBitmap, 85);
                    FileStream fstream = File.Create(output);
                    result.CopyTo(fstream);
                    fstream.Close();
                    result.Dispose();
                }
            }
        }

        private Stream CompressImage(Bitmap bmp, long quantity)
        {
            ImageCodecInfo jpgEncoder = this.GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quantity);
            myEncoderParameters.Param[0] = myEncoderParameter;
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, jpgEncoder, myEncoderParameters);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
