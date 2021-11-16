using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.PluginCore
{
    /// <summary>
    /// 執行結果的列舉
    /// </summary>
    public enum ExecuteResultEnum : int
    {
        /// <summary>
        /// 成功
        /// </summary>
        Successful = 1,
        /// <summary>
        /// 失敗
        /// </summary>
        Failed = 2,
        /// <summary>
        /// 強制執行失敗
        /// </summary>
        FailedWithForce = 3,
        /// <summary>
        /// 等待下次執行
        /// </summary>
        WaitNextTimeFetch = 4
    }
}
