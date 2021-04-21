using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 金鑰狀態列舉
    /// </summary>
    [Flags]
    public enum AuthorizeKeyStatusEnum : long
    {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 已停用
        /// </summary>
        Disable = 1 << 0
    }
}
