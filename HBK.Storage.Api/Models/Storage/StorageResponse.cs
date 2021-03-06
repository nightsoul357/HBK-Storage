using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.StorageCredentials;
using HBK.Storage.Api.Models.StorageGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.Storage
{
    /// <summary>
    /// 儲存個體回應內容
    /// </summary>
    public class StorageResponse
    {
        /// <summary>
        /// 儲存個體 ID
        /// </summary>
        public Guid StorageId { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public StorageTypeEnum Type { get; set; }
        /// <summary>
        /// 檔案大小限制(單位 Bytes)
        /// </summary>
        public long SizeLimit { get; set; }
        /// <summary>
        /// 存取儲存個體的驗證資訊
        /// </summary>
        public string Credentials { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime? UpdateDateTime { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public StorageStatusEnum[] Status { get; set; }
        /// <summary>
        /// 所在的儲存個體集合
        /// </summary>
        public StorageGroupResponse StorageGroupResponse { get; set; }
    }
}