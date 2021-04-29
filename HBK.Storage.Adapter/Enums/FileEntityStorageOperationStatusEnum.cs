using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 檔案位於儲存個體上的操作紀錄狀態列舉
    /// </summary>
    [Flags]
    public enum FileEntityStorageOperationStatusEnum : long
    {
        /// <summary>
        /// 無
        /// </summary>
        None = 0
    }
}
