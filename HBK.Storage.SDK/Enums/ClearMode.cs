using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// 清除模式列舉
    /// <br/>
    /// <br/>* **start** - 啟動
    /// <br/>* **stop** - 關閉
    /// <br/>
    /// </summary>
    public enum ClearMode
    {
        [System.Runtime.Serialization.EnumMember(Value = @"start")]
        Start = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"stop")]
        Stop = 1,
    }
}
