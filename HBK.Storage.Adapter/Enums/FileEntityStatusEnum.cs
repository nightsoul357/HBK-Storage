using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 檔案實體狀態列舉
    /// </summary>
    [Flags]
    public enum FileEntityStatusEnum : long
    {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 上傳中
        /// </summary>
        Uploading = 1 << 0,
        /// <summary>
        /// 處理中
        /// </summary>
        Processing  = 1 << 1
    }
}
