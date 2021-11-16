using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Sync.Model
{
    /// <summary>
    /// 任務管理者選項基底型別
    /// </summary>
    public abstract class TaskMangagerOptionBase
    {
        /// <summary>
        /// 取得或設定識別名稱
        /// </summary>
        public string Identity { get; set; }
        /// <summary>
        /// 取得或設定是否自動重試
        /// </summary>
        public bool IsAutoRetry { get; set; } = true;
        /// <summary>
        /// 取得或設定自動重試間隔
        /// </summary>
        public int AutoRetryInterval { get; set; } = 10000;
        /// <summary>
        /// 取得或設定執行間隔
        /// </summary>
        public int Interval { get; set; } = 1000;
    }
}
