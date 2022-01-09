using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// [Flags] 儲存個體群組狀態列舉
    /// <br/>
    /// <br/>* **disable** - 已停用
    /// <br/>
    /// </summary>
    public enum StorageGroupStatus
    {
        [System.Runtime.Serialization.EnumMember(Value = @"disable")]
        Disable = 0,
    }
}
