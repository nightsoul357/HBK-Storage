using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.PluginCore;
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

namespace HBK.Storage.ImageCompressPlugin
{
    public class ImageCompressTaskManager : PluginTaskManagerBase<ImageCompressTaskManager, ImageCompressTaskOptions>
    {
        public ImageCompressTaskManager(ILogger<ImageCompressTaskManager> logger, IServiceProvider serviceProvider)
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
                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return false;
                }


                using var bmp = new Bitmap(fileInfo.CreateReadStream());
                foreach (var compress in base.Options.CompressLevels)
                {
                    var result = this.CompressImage(bmp, compress.Quantity);
                    _ = storageProviderService
                    .UploadFileEntityAsync(taskModel.StorageProviderId,
                        null,
                        new FileEntity()
                        {
                            ExtendProperty = "",
                            MimeType = "image/jpeg",
                            Name = taskModel.FileEntity.Name + $"-{ compress.Name }",
                            Size = result.Length,
                            Status = FileEntityStatusEnum.None,
                            FileEntityTag = new List<FileEntityTag>() { new FileEntityTag()
                            {
                                Value = this.Options.Identity
                            },
                            new FileEntityTag()
                            {
                                Value = compress.Name
                            } },
                            ParentFileEntityID = taskModel.FileEntity.FileEntityId
                        },
                        result, this.Options.Identity).Result;
                }
            }
            return true;
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
