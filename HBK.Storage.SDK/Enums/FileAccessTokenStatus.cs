using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// [Flags] 檔案存取權杖狀態列舉
    /// <br/>
    /// <br/>* **revoke** - 撤銷
    /// <br/>
    /// </summary>
    public enum FileAccessTokenStatus
    {
        [System.Runtime.Serialization.EnumMember(Value = @"revoke")]
        Revoke = 0,
    }
}
