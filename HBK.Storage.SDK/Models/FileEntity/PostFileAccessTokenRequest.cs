using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.FileEntity
{
    /// <summary>
    /// 產生檔案存取權杖請求內容
    /// </summary>
    public class PostFileAccessTokenRequest
    {
        /// <summary>
        /// 強制指定檔案群組 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_group_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid? StorageGroupId { get; set; }

        /// <summary>
        /// 檔案實體 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("file_entity_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid? FileEntityId { get; set; }

        /// <summary>
        /// 允許 Tag 通過的格式
        /// </summary>
        [Newtonsoft.Json.JsonProperty("allow_tag_pattern", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string AllowTagPattern { get; set; }

        [Newtonsoft.Json.JsonProperty("file_access_token_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public FileAccessTokenType FileAccessTokenType { get; set; }

        /// <summary>
        /// 過期分鐘
        /// </summary>
        [Newtonsoft.Json.JsonProperty("expire_after_minutes", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int ExpireAfterMinutes { get; set; }

        /// <summary>
        /// 存取次數限制
        /// </summary>
        [Newtonsoft.Json.JsonProperty("access_times_limit", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? AccessTimesLimit { get; set; }

        /// <summary>
        /// 檔案處理器指示字串
        /// </summary>
        [Newtonsoft.Json.JsonProperty("handler_indicate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string HandlerIndicate { get; set; }
    }
}
