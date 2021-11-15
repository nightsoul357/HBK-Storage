using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Enums
{
    /// <summary>
    /// 加解密方向列舉
    /// </summary>
    public enum CryptoDirectionEnum : int
    {
        /// <summary>
        /// 加密
        /// </summary>
        Encrypt = 1,
        /// <summary>
        /// 解密
        /// </summary>
        Decrypt = 2
    }
}
