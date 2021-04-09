using HBK.Storage.Adapter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.StorageProvider
{
    /// <summary>
    /// 儲存服務回應內容
    /// </summary>
    public class StorageProviderResponse
    {
        /// <summary>
        /// 儲存服務 ID
        /// </summary>
        public Guid StorageProviderId { get; set; }
        /// <summary>
        /// 取得或設定名稱
        /// </summary>
        public string Name { get; set; }
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
        public StorageProviderStatusEnum[] Status { get; set; }
    }
}
