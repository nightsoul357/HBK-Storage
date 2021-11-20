using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Dashboard.DataSource.Models
{
    /// <summary>
    /// 本地類型的存取儲存個體的驗證資訊
    /// </summary>
    public class LocalStorageCredentials : StorageCredentialsBase
    {
        public LocalStorageCredentials()
        {
            base.Storage_type = StorageType.Local;
        }
        /// <summary>
        /// 取得或設定目錄
        /// </summary>
        [Newtonsoft.Json.JsonProperty("directory", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Directory { get; set; }
    }
}
