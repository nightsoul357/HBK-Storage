using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// 加密模式
    /// <br/>
    /// <br/>* **no_crypto** - 未加密
    /// <br/>* **aes** - AES 加密方式
    /// <br/>
    /// </summary>
    public enum CryptoMode
    {
        [System.Runtime.Serialization.EnumMember(Value = @"no_crypto")]
        NoCrypto = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"aes")]
        Aes = 1,
    }
}
