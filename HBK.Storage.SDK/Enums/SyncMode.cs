using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// 同步模式列舉
    /// <br/>
    /// <br/>* **always** - 總是同步
    /// <br/>* **never** - 永不同步
    /// <br/>* **policy** - 根據策略決定是否同步
    /// <br/>
    /// </summary>
    public enum SyncMode
    {
        [System.Runtime.Serialization.EnumMember(Value = @"always")]
        Always = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"never")]
        Never = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"policy")]
        Policy = 2,
    }
}
