using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.FileEntity
{
    /// <summary>
    /// Child 的檔案實體回應內容
    /// </summary>
    public class ChildFileEntityResponse
    {
        /// <summary>
        /// 根檔案 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("root_file_entity_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid? RootFilEntityId { get; set; }

        /// <summary>
        /// 與根檔案相差的層數
        /// </summary>
        [Newtonsoft.Json.JsonProperty("child_level", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? ChildLevel { get; set; }

        /// <summary>
        /// 檔案實體 ID
        /// </summary>
        [Newtonsoft.Json.JsonProperty("file_entity_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Guid FileEntityId { get; set; }

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
        /// 檔案標籤列表
        /// </summary>
        [Newtonsoft.Json.JsonProperty("tags", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<string> Tags { get; set; }

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

        [Newtonsoft.Json.JsonProperty("access_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public AccessType AccessType { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("create_date_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset CreateDateTime { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("update_date_time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset? UpdateDateTime { get; set; }

        /// <summary>
        /// 加密使用的 Key
        /// </summary>
        [Newtonsoft.Json.JsonProperty("crypto_key", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CryptoKey { get; set; }

        /// <summary>
        /// 加密使用的 Iv
        /// </summary>
        [Newtonsoft.Json.JsonProperty("crypto_iv", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CryptoIv { get; set; }

        [Newtonsoft.Json.JsonProperty("crypto_mode", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public CryptoMode CryptoMode { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public System.Collections.Generic.ICollection<FileEntityStatus> Status { get; set; }

        /// <summary>
        /// 取得或設定存在的儲存集合清單
        /// </summary>
        [Newtonsoft.Json.JsonProperty("file_entity_storage_responses", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<FileEntityStorageResponse> FileEntityStorageResponses { get; set; }
    }
}
