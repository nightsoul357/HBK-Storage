using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.StorageGroup
{
    /// <summary>
    /// 儲存個體集合回應內容
    /// </summary>
    public class StorageGroupResponse
    {
        /// <summary>
        /// 儲存個體群組 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_group_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid Storage_group_id { get; set; }

        /// <summary>
        /// 所屬儲存服務 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_provider_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid? Storage_provider_id { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>
        /// 類型
        /// </summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public StorageType Type { get; set; }

        /// <summary>
        /// 同步模式
        /// </summary>
        [Newtonsoft.Json.JsonProperty("sync_mode", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public SyncMode SyncMode { get; set; }

        /// <summary>
        /// 同步策略
        /// </summary>
        [Newtonsoft.Json.JsonProperty("sync_policy", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SyncPolicy SyncPolicy { get; set; }

        /// <summary>
        /// 清除模式
        /// </summary>
        [Newtonsoft.Json.JsonProperty("clear_mode", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ClearMode ClearMode { get; set; }

        /// <summary>
        /// 清除策略
        /// </summary>
        [Newtonsoft.Json.JsonProperty("clear_policy", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ClearPolicy ClearPolicy { get; set; }

        /// <summary>
        /// 上傳優先度
        /// </summary>
        [Newtonsoft.Json.JsonProperty("upload_priority", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int UploadPriority { get; set; }

        /// <summary>
        /// 下載優先度
        /// </summary>
        [Newtonsoft.Json.JsonProperty("download_priority", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int DownloadPriority { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("create_date_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset CreateDateTime { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("update_date_time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset? UpdateDateTime { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ICollection<StorageGroupStatus> Status { get; set; }
    }
}
