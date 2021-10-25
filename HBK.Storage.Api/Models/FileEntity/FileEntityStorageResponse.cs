using HBK.Storage.Adapter.Enums;
using HBK.Storage.Api.Models.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileEntity
{
    /// <summary>
    /// 檔案實體於儲存個體上的回應內容
    /// </summary>
    public class FileEntityStorageResponse : StorageSummaryResponse
    {
        /// <summary>
        /// 檔案於儲存個體上的狀態
        /// </summary>
        public FileEntityStorageStatusEnum[] FileEntityStorageStatus { get; set; }
        /// <summary>
        /// 檔案於儲存個體上的建立時間
        /// </summary>
        public DateTime FileEntityStorageCreateDateTime { get; set; }
        /// <summary>
        /// 檔案於儲存個體上的最後更新時間
        /// </summary>
        public DateTime? FileEntityStorageUpdateDateTime { get; set; }
        /// <summary>
        /// 檔案於儲存個體上的建立者識別名稱
        /// </summary>
        public string CreatorIdentity { get; set; }
    }
}
