using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.StorageCredentials
{
    /// <summary>
    /// Amazon S3 使用的驗證資訊
    /// </summary>
    public class AmazonS3StorageCredentials : StorageCredentialsBase
    {
        /// <summary>
        /// 取得或設定存取金鑰
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        /// 取得或設定存取密鑰
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 取得或設定 Bucket 名稱
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// 取得或設定地區
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 取得或設定位置
        /// </summary>
        public string ServiceURL { get; set; }
        /// <summary>
        /// 取得或設定 ForcePathStyle
        /// </summary>
        public bool ForcePathStyle { get; set; }
    }
}
