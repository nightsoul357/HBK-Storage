using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 標籤驗證模式列舉
    /// </summary>
    public enum TagMatchModeEnum : int
    {
        /// <summary>
        /// 無
        /// </summary>
        None = 0,
        /// <summary>
        /// 所有
        /// </summary>
        All = 1,
        /// <summary>
        /// 任一
        /// </summary>
        Any = 2
    }
}
