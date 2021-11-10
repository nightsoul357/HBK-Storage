using HBK.Storage.Adapter.Enums;
using HBK.Storage.Api.Models.Storage;
using HBK.Storage.Api.Models.StorageGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileEntity
{
    /// <summary>
    /// 檔案實體回應內容
    /// </summary>
    public class FileEntityResponse
    {
        /// <summary>
        /// 檔案實體 ID
        /// </summary>
        public Guid FileEntityId { get; set; }
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 檔案大小(單位 Bytes)
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// 檔案標籤列表
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 檔案擴充描述
        /// </summary>
        public string ExtendProperty { get; set; }
        /// <summary>
        /// 檔案的網際網路媒體型式
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// 存取模式
        /// </summary>
        public AccessTypeEnum AccessType { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateDateTime { get; internal set; }
        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime? UpdateDateTime { get; internal set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public FileEntityStatusEnum[] Status { get; set; }
        /// <summary>
        /// 取得或設定存在的儲存集合清單
        /// </summary>
        public List<FileEntityStorageResponse> FileEntityStorageResponses { get; set; }
    }
}
