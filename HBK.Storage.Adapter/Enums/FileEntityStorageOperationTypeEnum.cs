using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 檔案位於儲存個體上的操作紀錄類型
    /// </summary>
    public enum FileEntityStorageOperationTypeEnum : int
    {
        /// <summary>
        /// 同步失敗
        /// </summary>
        SyncFail = 1,
        /// <summary>
        /// 同步成功
        /// </summary>
        SyncSuccessfully = 2,
        /// <summary>
        /// 取得資料失敗
        /// </summary>
        FetchFail = 3,
        /// <summary>
        /// 取得資料成功
        /// </summary>
        FetchSuccessfully = 4,
        /// <summary>
        /// 存取失敗
        /// </summary>
        AccessFail = 5,
        /// <summary>
        /// 存取成功
        /// </summary>
        AccessSuccessfully = 6,
    }
}
