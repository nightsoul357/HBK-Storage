using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.StorageGroup
{
    /// <summary>
    /// 測試同步策略請求內容
    /// </summary>
    public class TestSyncPolicyRequest
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// 檔案大小(單位 Bytes)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("size", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Size { get; set; }

        /// <summary>
        /// 檔案擴充描述
        /// </summary>
        [Newtonsoft.Json.JsonProperty("extend_property", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ExtendProperty { get; set; }

        /// <summary>
        /// 檔案的網際網路媒體型式
        /// </summary>
        [Newtonsoft.Json.JsonProperty("mime_type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string MimeType { get; set; }

        /// <summary>
        /// 檔案的標籤清單
        /// </summary>
        [Newtonsoft.Json.JsonProperty("tags", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<string> Tags { get; set; }

        [Newtonsoft.Json.JsonProperty("sync_policy", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SyncPolicyRequest SyncPolicy { get; set; }
    }
}
