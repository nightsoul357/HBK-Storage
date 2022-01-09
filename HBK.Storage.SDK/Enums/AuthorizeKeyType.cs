using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// 金鑰類型列舉
    /// <br/>
    /// <br/>* **root** - 最高權限的金鑰，可以存取所有服務
    /// <br/>* **normal** - 一般的金鑰，根據 Scope 的定義存取服務
    /// <br/>
    /// </summary>
    public enum AuthorizeKeyType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"root")]
        Root = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"normal")]
        Normal = 1,
    }
}
