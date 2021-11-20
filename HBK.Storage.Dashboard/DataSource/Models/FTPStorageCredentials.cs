using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Dashboard.DataSource.Models
{
    /// <summary>
    /// FTP 類型的存取儲存個體的驗證資訊
    /// </summary>
    public class FTPStorageCredentials : StorageCredentialsBase
    {
        public FTPStorageCredentials()
        {
            base.Storage_type = StorageType.Ftp;
        }
        /// <summary>
        /// 取得或設定 Url
        /// </summary>
        [Newtonsoft.Json.JsonProperty("url", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("username", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Username { get; set; }
        /// <summary>
        /// 取得或設定密碼
        /// </summary>
        [Newtonsoft.Json.JsonProperty("password", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Password { get; set; }
    }
}
