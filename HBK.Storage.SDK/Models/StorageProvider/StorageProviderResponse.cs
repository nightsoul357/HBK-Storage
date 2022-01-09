using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.StorageProvider
{
    /// <summary>
    /// 儲存服務回應內容
    /// </summary>
    public class StorageProviderResponse
    {
        /// <summary>
        /// 儲存服務 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_provider_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Guid Storage_provider_id { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("create_date_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset Create_date_time { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("update_date_time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? Update_date_time { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ICollection<StorageProviderStatus> Status { get; set; }
    }
}
