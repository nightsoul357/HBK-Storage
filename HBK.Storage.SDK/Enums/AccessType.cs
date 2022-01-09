using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// 存取模式列舉
    /// <br/>
    /// <br/>* **private** - 私人
    /// <br/>* **public** - 公開
    /// <br/>
    /// </summary>
    public enum AccessType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"private")]
        Private = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"public")]
        Public = 1,
    }
}
