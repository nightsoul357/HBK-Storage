using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Sync.Model
{
    /// <summary>
    /// 同步任務管理者選項
    /// </summary>
    public class SyncTaskManagerOption
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public SyncTaskManagerOption()
        {
            this.StorageProviderIds = new List<Guid>();
        }
        /// <summary>
        /// 取得或設定識別名稱
        /// </summary>
        public string Identity { get; set; }
        /// <summary>
        /// 取得或設定工作數量上限
        /// </summary>
        public int TaskLimit { get; set; }
        /// <summary>
        /// 取得或設定作用的儲存服務 ID 清單
        /// </summary>
        public List<Guid> StorageProviderIds { get; set; }
        /// <summary>
        /// 取得或設定是否在所有儲存服務上執行
        /// </summary>
        public bool IsExecutOnAllStoragProvider { get; set; }
        /// <summary>
        /// 取得或設定檔案實體流水號除數
        /// </summary>
        public int FileEntityNoDivisor { get; set; }
        /// <summary>
        /// 取得或設定檔案流水號餘數
        /// </summary>
        public int FileEntityNoRemainder { get; set; }
        /// <summary>
        /// 取得或設定單次取得同步任務數量上限
        /// </summary>
        public int FetchLimit { get; set; }
    }
}
