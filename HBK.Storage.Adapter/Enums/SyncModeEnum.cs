using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 同步策略列舉
    /// </summary>
    public enum SyncModeEnum : int
    {
        /// <summary>
        /// 總是同步
        /// </summary>
        Always = 1,
        /// <summary>
        /// 永不同步
        /// </summary>
        Never = 2,
        /// <summary>
        /// 根據策略決定是否同步
        /// </summary>
        Policy = 3
    }
}
