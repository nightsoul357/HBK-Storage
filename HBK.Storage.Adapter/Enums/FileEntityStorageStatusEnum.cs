using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    ///  檔案位於儲存個體上的橋接資訊狀態列舉
    /// </summary>
    [Flags]
    public enum FileEntityStorageStatusEnum : long
    {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 正在同步
        /// </summary>
        Syncing = 1 << 0,
        /// <summary>
        /// 同步失敗
        /// </summary>
        SyncFail = 1 << 1
    }
}