﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// [Flags] 金鑰狀態列舉
    /// <br/>
    /// <br/>* **disable** - 已停用
    /// <br/>
    /// </summary>
    public enum AuthorizeKeyStatus
    {
        [System.Runtime.Serialization.EnumMember(Value = @"disable")]
        Disable = 0,
    }
}
