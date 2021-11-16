using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 加密模式
    /// </summary>
    public enum CryptoModeEnum : int
    {
        /// <summary>
        /// 未加密
        /// </summary>
        NoCrypto = 1,
        /// <summary>
        /// AES 加密方式
        /// </summary>
        AES = 2
    }
}
