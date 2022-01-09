using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.FileEntity
{
    /// <summary>
    /// 檔案存取權杖回應內容
    /// </summary>
    public class FileAccessTokenResponse
    {
        /// <summary>
        /// 檔案存取權杖 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("file_access_token_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid FileAccessTokenId { get; set; }

        /// <summary>
        /// 存取對象的儲存服務 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_provider_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid StorageProviderId { get; set; }

        /// <summary>
        /// 強制指定的存取對象的儲存個體集合 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_group_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid? StorageGroupId { get; set; }

        /// <summary>
        /// 檔案實體 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("file_entity_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid? FileEntityId { get; set; }

        /// <summary>
        /// 權杖
        /// </summary>
        [Newtonsoft.Json.JsonProperty("token", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Token { get; set; }

        /// <summary>
        /// 存取次數限制
        /// </summary>
        [Newtonsoft.Json.JsonProperty("access_times_limit", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int AccessTimesLimit { get; set; }

        /// <summary>
        /// 存取次數
        /// </summary>
        [Newtonsoft.Json.JsonProperty("access_times", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int AccessTimes { get; set; }

        /// <summary>
        /// 過期時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("expire_date_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset ExpireDateTime { get; set; }

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

        /// <summary>
        /// 狀態
        /// </summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ICollection<FileAccessTokenStatus> Status { get; set; }
    }
}
