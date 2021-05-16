﻿using HBK.Storage.Adapter.Enums;
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
        public VideoSeekPreviewTaskManager(ILogger<VideoSeekPreviewTaskManager> logger, IServiceProvider serviceProvider)
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

                base._logger.LogInformation("[{0}] 開始處理檔案 ID {1} 的 {2} 之 Seek Preview", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);

                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return false;
                }

                Guid taskId = Guid.NewGuid();
                string workingDirectory = Path.Combine(base.Options.WorkingDirectory, taskId.ToString());
                Directory.CreateDirectory(workingDirectory);

                string videoFileName = Path.Combine(workingDirectory, Guid.NewGuid() + Path.GetExtension(taskModel.FileEntity.Name));
                base.DownloadFileEntity(fileInfo, taskModel.FileEntity, videoFileName);

                var videoFile = new MediaFile { Filename = videoFileName };
                string newFFmpegLocation = Path.Combine(workingDirectory, "ffmpeg.exe");
                File.Copy(base.Options.FFmpegLocation, newFFmpegLocation);

                using var engine = new Engine(newFFmpegLocation);
                engine.GetMetadata(videoFile);
                List<string> previewsFiles = new List<string>();
                for (int i = 0; i < videoFile.Metadata.Duration.TotalSeconds; i++)
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
                    previewsFiles.Add(newFile);
                    base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 的第 {3} 張 Seek Preview 產生完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name, i);
                }

                List<FileEntity> processFileEntities = new List<FileEntity>();
                for (int i = 0; i < previewsFiles.Count; i++)
                {
                    using (var fstream = File.Open(previewsFiles[i], FileMode.Open))
                    {
                        var fileEntity = storageProviderService
                            .UploadFileEntityAsync(taskModel.StorageProviderId,
                                null,
                                new FileEntity()
                                {
                                    MimeType = "image/jpeg",
                                    Name = Path.GetFileNameWithoutExtension(taskModel.FileEntity.Name) + $"-Seek-{i}.jpeg",
                                    Size = fstream.Length,
                                    Status = FileEntityStatusEnum.Processing,
                                    FileEntityTag = new List<FileEntityTag>() {
                                        new FileEntityTag()
                                        {
                                            Value = this.Options.Identity
                                        },
                                        new FileEntityTag()
                                        {
                                            Value = $"Seek-{i}"
                                        }
                                    },
                                    ParentFileEntityID = taskModel.FileEntity.FileEntityId
                                },
                                fstream, this.Options.Identity).Result;

                        processFileEntities.Add(fileEntity);
                        base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 的第 {3} 張 Seek Preview 上傳完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name, i);
                    }
                }

                processFileEntities.ForEach(x => x.Status = x.Status & ~FileEntityStatusEnum.Processing);
                fileEntityService.UpdateBatchAsync(processFileEntities).Wait();

                DirectoryOperator.DeleteSaftey(workingDirectory, true);

                base._logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 之 Seek Preview 處理完成", base.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name);

                return true;
            }
        }

        public void ResizeImage(string source, string output, int newWidth)
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