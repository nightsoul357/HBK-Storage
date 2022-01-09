using HBK.Storage.SDK.Enums;
using HBK.Storage.SDK.Models.StorageGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.FileEntity
{
    /// <summary>
    /// 檔案實體於儲存個體上的回應內容
    /// </summary>
    public class FileEntityStorageResponse
    {
        /// <summary>
        /// 檔案於儲存個體上的狀態
        /// </summary>
        [Newtonsoft.Json.JsonProperty("file_entity_storage_status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ICollection<FileEntityStorageStatus> FileEntityStorageStatus { get; set; }

        /// <summary>
        /// 檔案於儲存個體上的建立時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("file_entity_storage_create_date_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset FileEntityStorageCreateDateTime { get; set; }

        /// <summary>
        /// 檔案於儲存個體上的最後更新時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("file_entity_storage_update_date_time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset? FileEntityStorageUpdateDateTime { get; set; }

        /// <summary>
        /// 檔案於儲存個體上的建立者識別名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("creator_identity", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CreatorIdentity { get; set; }

        /// <summary>
        /// 儲存個體 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid StorageId { get; set; }

        /// <summary>名稱</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public StorageType Type { get; set; }

        /// <summary>檔案大小限制(單位 Bytes)</summary>
        [Newtonsoft.Json.JsonProperty("size_limit", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long Size_limit { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("create_date_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset CreateDateTime { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("update_date_time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? UpdateDateTime { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ICollection<StorageStatus> Status { get; set; }

        /// <summary>
        /// 所屬儲存個體集合回應內容
        /// </summary>
        [Newtonsoft.Json.JsonProperty("storage_group_response", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public StorageGroupResponse StorageGroupResponse { get; set; }
    }
}
