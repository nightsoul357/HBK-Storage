using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.AuthorizeKey
{
    /// <summary>
    /// 驗證金鑰使用範圍回應內容
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v13.0.0.0)")]
    public class AuthorizeKeyScopeResponse
    {
        /// <summary>
        /// 儲存服務 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_provider_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Guid StorageProviderId { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("create_date_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset CreateDateTime { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("update_date_time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? UpdateDateTime { get; set; }

        [Newtonsoft.Json.JsonProperty("allow_operation_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public AuthorizeKeyScopeOperationType AllowOperationType { get; set; }
    }
}
