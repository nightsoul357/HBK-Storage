using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Models
{
    /// <summary>
    /// 檔案位於儲存個體上的資訊服務選項
    /// </summary>
    public class FileEntityStorageServiceOption
    {
        /// <summary>
        /// Fetch 檔案失敗導致檔案關閉的嘗試次數
        /// </summary>
        public int FetchFailToCloseFileCount { get; set; } = 3;
        /// <summary>
        /// Fetch 檔案失敗導致關閉儲存個體的嘗試次數
        /// </summary>
        public int FetchFailToCloseStorgeCount { get; set; } = 10;
        /// <summary>
        /// 同步至目標儲存個體失敗導致關閉目標儲存個體的嘗試次數
        /// </summary>
        public int SyncTargetFailtToCloseStorageCount { get; set; } = 10;
    }
}
