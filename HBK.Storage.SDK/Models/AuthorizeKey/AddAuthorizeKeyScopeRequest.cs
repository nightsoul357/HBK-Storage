using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.AuthorizeKey
{
    /// <summary>
    /// 新增驗證金鑰使用範圍請求內容
    /// </summary>
    public partial class AddAuthorizeKeyScopeRequest
    {
        /// <summary>
        /// 儲存服務 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_provider_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid StorageProviderId { get; set; }

        [Newtonsoft.Json.JsonProperty("allow_operation_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public AuthorizeKeyScopeOperationType AllowOperationType { get; set; }
    }
}
