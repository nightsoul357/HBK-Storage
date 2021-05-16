using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.StorageGroup
{
    /// <summary>
    /// 儲存個體集合回應內容
    /// </summary>
    public class StorageGroupResponse
    {
        /// <summary>
        /// 儲存個體群組 ID
        /// </summary>
        public Guid StorageGroupId { get; set; }
        /// <summary>
        /// 所屬儲存服務 ID
        /// </summary>
        public Guid? StorageProviderId { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public StorageTypeEnum Type { get; set; }
        /// <summary>
        /// 同步模式
        /// </summary>
        public SyncModeEnum SyncMode { get; set; }
        /// <summary>
        /// 同步策略
        /// </summary>
        public SyncPolicy SyncPolicy { get; set; }
        /// <summary>
        /// 上傳優先度
        /// </summary>
        public int UploadPriority { get; set; }
        /// <summary>
        /// 下載優先度
        /// </summary>
        public int DownloadPriority { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTimeOffset CreateDateTime { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTimeOffset? UpdateDateTime { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public StorageGroupStatusEnum[] Status { get; set; }
    }
}
