using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Dashboard.DataSource.Models
{
    /// <summary>
    /// Google Drive 使用的驗證資訊
    /// </summary>
    public class GoogleDriveCredentials : StorageCredentialsBase
    {
        public GoogleDriveCredentials()
        {
            base.Storage_type = StorageType.Google_drive;
        }
        /// <summary>
        /// 取得或設定父資料夾 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("parent", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Parent { get; set; }
        /// <summary>
        /// 取得或設定存取金鑰
        /// </summary>
        [Newtonsoft.Json.JsonProperty("client_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ClientId { get; set; }
        /// <summary>
        /// 取得或設定存取密鑰
        /// </summary>
        [Newtonsoft.Json.JsonProperty("client_secret", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ClientSecret { get; set; }
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("user", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string User { get; set; }
        /// <summary>
        /// 取得或設定 Token 資訊
        /// </summary>
        [Newtonsoft.Json.JsonProperty("tokens", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Dictionary<string,string> Tokens { get; set; }
    }
}
