using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// 儲存服務狀態列舉
    /// <br/>
    /// <br/>* **none** - 無
    /// <br/>* **disable** - 已停用
    /// <br/>
    /// </summary>
    public enum StorageProviderStatus
    {
        /// <summary>
        /// 無
        /// </summary>
        [System.Runtime.Serialization.EnumMember(Value = @"none")]
        None = 0,

        /// <summary>
        /// 已停用
        /// </summary>
        [System.Runtime.Serialization.EnumMember(Value = @"disable")]
        Disable = 1
    }
}
