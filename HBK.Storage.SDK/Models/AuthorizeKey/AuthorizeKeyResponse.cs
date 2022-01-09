using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.AuthorizeKey
{
    /// <summary>
    /// 驗證金鑰回應內容
    /// </summary>
    public class AuthorizeKeyResponse
    {
        /// <summary>
        /// 金鑰 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("authorize_key_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid AuthorizeKeyId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// 金鑰值
        /// </summary>
        [Newtonsoft.Json.JsonProperty("key_value", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string KeyValue { get; set; }

        /// <summary>
        /// 取得建立時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("create_date_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset CreateDateTime { get; set; }

        /// <summary>
        /// 取得最後更新時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("update_date_time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset? UpdateDateTime { get; set; }

        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public AuthorizeKeyType Type { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ICollection<AuthorizeKeyStatus> Status { get; set; }

        /// <summary>
        /// 驗證用的金鑰資訊清單
        /// </summary>
        [Newtonsoft.Json.JsonProperty("authorize_key_scope_responses", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<AuthorizeKeyScopeResponse> AuthorizeKeyScopeResponses { get; set; }
    }
}
