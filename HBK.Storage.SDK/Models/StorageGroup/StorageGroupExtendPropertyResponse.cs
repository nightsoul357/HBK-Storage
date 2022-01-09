using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.StorageGroup
{
    /// <summary>
    /// 儲存群組額外資訊回應內容
    /// </summary>
    public class StorageGroupExtendPropertyResponse
    {
        /// <summary>
        /// 大小限制(Bytes)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("size_limit", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long? SizeLimit { get; set; }

        /// <summary>
        /// 已使用大小(Bytes)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("used_size", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long? UsedSize { get; set; }

        /// <summary>
        /// 儲存個體群組 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_group_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid StorageGroupId { get; set; }

        /// <summary>
        /// 所屬儲存服務 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_provider_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid? StorageProviderId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public StorageType Type { get; set; }

        [Newtonsoft.Json.JsonProperty("sync_mode", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public SyncMode SyncMode { get; set; }

        [Newtonsoft.Json.JsonProperty("sync_policy", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SyncPolicy SyncPolicy { get; set; }

        [Newtonsoft.Json.JsonProperty("clear_mode", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ClearMode ClearMode { get; set; }

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