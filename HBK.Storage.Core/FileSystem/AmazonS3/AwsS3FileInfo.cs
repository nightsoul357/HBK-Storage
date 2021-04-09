using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.AmazonS3
{
    /// <summary>
    /// AWS S3 檔案資訊
    /// </summary>
    public class AwsS3FileInfo : AsyncFileInfo
    {
        private readonly AmazonS3Client _client;
        private readonly DateTime? _lastModified;

        /// <summary>
        /// 取得檔案修改時間
        /// </summary>
        public override DateTimeOffset LastModified => _lastModified ?? throw new FileNotFoundException();

        /// <summary>
        /// 取得 Bucket 名稱
        /// </summary>
        public string BucketName { get; }

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="client">AWS S3 客戶端</param>
        /// <param name="key">檔案主鍵</param>
        /// <param name="bucketName">bucket 名稱</param>
        /// <param name="physicalPath">檔案路徑</param>
        /// <param name="metadata">中繼資料</param>
        internal AwsS3FileInfo(AmazonS3Client client, string key, string bucketName, string physicalPath, GetObjectMetadataResponse metadata)
        {
            _client = client;
            this.IsDirectory = false;
            this.Exists = metadata != null;
            this.Length = metadata.ContentLength;
            this.Name = key;
            this.BucketName = bucketName;
            this.PhysicalPath = physicalPath;
            _lastModified = metadata.LastModified;
        }

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="client">AWS S3 客戶端</param>
        /// <param name="key">檔案主鍵</param>
        /// <param name="bucketName">bucket 名稱</param>
        /// <param name="physicalPath">檔案路徑</param>
        /// <param name="s3Object">AWS S3 物件</param>
        internal AwsS3FileInfo(AmazonS3Client client, string key, string bucketName, string physicalPath, S3Object s3Object)
        {
            _client = client;
            this.Exists = s3Object != null;
            this.Length = s3Object.Size;
            this.Name = key;
            this.BucketName = bucketName;
            this.PhysicalPath = physicalPath;
            _lastModified = s3Object.LastModified;
        }

        /// <summary>
        /// 取得檔案資料
        /// </summary>
        /// <returns></returns>
        public override async Task<Stream> CreateReadStreamAsync()
        {
            if (!this.Exists)
            {
                throw new FileNotFoundException();
            }

            var response = await _client.GetObjectAsync(new GetObjectRequest()
            {
                BucketName = this.BucketName,
                Key = this.Name
            });
            return response.ResponseStream;
        }
    }
}
