using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Web.DataSource.Models
{
    /// <summary>
    /// Amazon S3 使用的驗證資訊
    /// </summary>
    public class AmazonS3StorageCredentials : StorageCredentialsBase
    {
        /// <summary>
        /// 取得或設定存取金鑰
        /// </summary>
        [Newtonsoft.Json.JsonProperty("access_key", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string AccessKey { get; set; }
        /// <summary>
        /// 取得或設定存取密鑰
        /// </summary>
        [Newtonsoft.Json.JsonProperty("secret_key", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string SecretKey { get; set; }
        /// <summary>
        /// 取得或設定Bucket 名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("bucket_name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string BucketName { get; set; }
        /// <summary>
        /// 取得或設定地區
        /// </summary>
        [Newtonsoft.Json.JsonProperty("region", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Region { get; set; }
        /// <summary>
        /// 取得或設定位置
        /// </summary>
        [Newtonsoft.Json.JsonProperty("service_url", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ServiceURL { get; set; }
        /// <summary>
        /// 取得或設定 ForcePathStyle
        /// </summary>
        [Newtonsoft.Json.JsonProperty("force_path_style", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool ForcePathStyle { get; set; }
    }
}
