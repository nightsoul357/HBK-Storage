using HBK.Storage.Adapter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Cryptography
{
    /// <summary>
    /// 加密方法提供者
    /// </summary>
    public interface ICryptoProvider
    {
        /// <summary>
        /// 產生亂數 Key
        /// </summary>
        /// <returns></returns>
        byte[] GenerateRandomKey();
        /// <summary>
        /// 產生亂數 Iv
        /// </summary>
        /// <returns></returns>
        byte[] GenerateRandomIv();
        /// <summary>
        /// 產生加密用的方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        ICryptoTransform GenerateEncryptTransform(byte[] key, byte[] iv);
        /// <summary>
        /// 產生解密用的方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        ICryptoTransform GenerateDecryptTransform(byte[] key, byte[] iv);
        /// <summary>
        /// 取得加密方式
        /// </summary>
        CryptoModeEnum CryptoMode { get; }
    }
}
