using Amazon;
using Amazon.S3;
using HBK.Storage.Adapter.StorageCredentials;
using HBK.Storage.Core.FileSystem.AmazonS3;
using HBK.Storage.Core.FileSystem.FTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem
{
    /// <summary>
    /// 檔案系統工廠
    /// </summary>
    public class FileSystemFactory
    {
        /// <summary>
        /// 取得指定儲存個體之儲存服務提供者
        /// </summary>
        /// <param name="storage">儲存個體</param>
        /// <returns></returns>
        public IAsyncFileProvider GetAsyncFileProvider(Adapter.Storages.Storage storage)
        {
            switch (storage.Type)
            {
                case Adapter.Enums.StorageTypeEnum.FTP:
                    {
                        FTPStorageCredentials credentials = (FTPStorageCredentials)storage.Credentials;
                        return new FTPFileProvider(storage.Name, credentials.Url, credentials.Username, credentials.Password);
                    }
                case Adapter.Enums.StorageTypeEnum.AmazonS3:
                    {
                        AmazonS3StorageCredentials credentials = (AmazonS3StorageCredentials)storage.Credentials;
                        AmazonS3Config config = new AmazonS3Config()
                        {
                            RegionEndpoint = RegionEndpoint.GetBySystemName(credentials.Region),
                            ForcePathStyle = credentials.ForcePathStyle
                        };
                        if (!String.IsNullOrWhiteSpace(credentials.ServiceURL))
                        {
                            config.ServiceURL = credentials.ServiceURL;
                            config.UseHttp = !credentials.ServiceURL.StartsWith("https");
                        }
                        return new AwsS3FileProvider(storage.Name, credentials.AccessKey, credentials.SecretKey, credentials.BucketName, config);
                    }
                default:
                    throw new NotImplementedException($"尚未實作取得 { storage.Type } 儲存服務提供者的方式");
            }
        }
    }
}
