using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 檔案存取權杖狀態列舉
    /// </summary>
    [Flags]
    public enum FileAccessTokenStatusEnum : long
    {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 撤銷
        /// </summary>
        Revoke = 1 << 0,
    }
}
