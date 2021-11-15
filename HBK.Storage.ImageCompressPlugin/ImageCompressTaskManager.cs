using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.Cryptography;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.ImageCompressPlugin.Models;
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
        private readonly List<StorageTypeEnum> _storageTypes;
        public ImageCompressTaskManager(ILogger<ImageCompressTaskManager> logger, IServiceProvider serviceProvider)
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
            base.LogInformation(taskModel, null, "任務開始 - 壓縮圖片");

            using (var scope = base._serviceProvider.CreateScope())
            {
                var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                var fileEntityStorage = storageProviderService.GetFileEntityStorageAsync(taskModel.StorageProviderId, null, taskModel.FileEntity.FileEntityId, _storageTypes).Result;
                var cryptoProviders = scope.ServiceProvider.GetRequiredService<IEnumerable<ICryptoProvider>>();

                base.RemoveResidueFileEntity(fileEntityService, taskModel);

                IAsyncFileInfo fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId).Result;
                if (fileInfo == null)
                {
                    return ExecuteResultEnum.Failed;
                }

                var fileStream = fileInfo.CreateReadStream();
                if (taskModel.FileEntity.CryptoMode != CryptoModeEnum.NoCrypto)
                {
                    fileStream = new CryptoStream(fileStream, cryptoProviders.FirstOrDefault(x => x.CryptoMode == taskModel.FileEntity.CryptoMode), taskModel.FileEntity.CryptoKey, taskModel.FileEntity.CryptoIv);
                }
                using var bmp = new Bitmap(fileStream);
                List<FileEntity> processFileEntities = new List<FileEntity>();
                foreach (var compress in base.Options.CompressLevels)
                {
                    var result = this.CompressImage(bmp, compress.Quantity);
                    var compressFileEntity = storageProviderService
                    .UploadFileEntityAsync(taskModel.StorageProviderId,
                        null,
                        new FileEntity()
                        {
                            MimeType = "image/jpeg",
                            Name = Path.GetFileNameWithoutExtension(taskModel.FileEntity.Name) + $"-{ compress.Name }.jpeg",
                            Size = result.Length,
                            Status = FileEntityStatusEnum.Processing,
                            FileEntityTag = new List<FileEntityTag>() {
                                new FileEntityTag()
                                {
                                    Value = this.Options.Identity
                                },
                                new FileEntityTag()
                                {
                                    Value = compress.Name
                                }
                            },
                            ParentFileEntityID = taskModel.FileEntity.FileEntityId,
                            CryptoMode = taskModel.FileEntity.CryptoMode
                        },
                        result, this.Options.Identity, _storageTypes).Result;

                    processFileEntities.Add(compressFileEntity);
                }

                base.LogInformation(taskModel, null, "任務結束 - 壓縮圖片");
                processFileEntities.ForEach(x => x.Status = x.Status & ~FileEntityStatusEnum.Processing);
                fileEntityService.UpdateBatchAsync(processFileEntities).Wait();
            }
            return ExecuteResultEnum.Successful;
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