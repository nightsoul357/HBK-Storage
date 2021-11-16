using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Web.DataSource.Models
{
    /// <summary>
    /// Mega 使用的驗證資訊
    /// </summary>
    public class MegaStorageCredentials : StorageCredentialsBase
    {
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
        /// <summary>
        /// 取得或設定父系資料夾 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("parent_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ParentId { get; set; }
    }
}
