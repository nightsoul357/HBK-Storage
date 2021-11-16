using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Sync.Model
{
    /// <summary>
    /// 過期檔案處理任務管理者選項
    /// </summary>
    public class ExpireFileEntityTaskManagerOption : TaskMangagerOptionBase
    {
        /// <summary>
        /// 取得或設定檔案實體流水號除數
        /// </summary>
        public int FileEntityNoDivisor { get; set; }
        /// <summary>
        /// 取得或設定檔案流水號餘數
        /// </summary>
        public int FileEntityNoRemainder { get; set; }
        /// <summary>
        /// 取得或設定單次取得刪除檔案實體任務數量上限
        /// </summary>
        public int FetchLimit { get; set; }
    }
}
