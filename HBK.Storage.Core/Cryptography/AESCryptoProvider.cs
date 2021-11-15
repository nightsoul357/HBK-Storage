using HBK.Storage.Adapter.Enums;
using HBK.Storage.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Cryptography
{
    /// <summary>
    /// 使用 AES 的加密方法提供者
    /// </summary>
    public class AESCryptoProvider : ICryptoProvider
    {

        /// <inheritdoc/>
        public ICryptoTransform GenerateDecryptTransform(byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.None;
                aes.Mode = CipherMode.CBC;
                return aes.CreateEncryptor(key, iv);
            }
        }

        /// <inheritdoc/>
        public ICryptoTransform GenerateEncryptTransform(byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.None;
                aes.Mode = CipherMode.CBC;
                return aes.CreateDecryptor(key, iv);
            }
        }

        /// <inheritdoc/>
        public byte[] GenerateRandomIv()
        {
            return Encoding.UTF8.GetBytes(StringHelper.GetRandomString(16));
        }

        /// <inheritdoc/>
        public byte[] GenerateRandomKey()
        {
            return Encoding.UTF8.GetBytes(StringHelper.GetRandomString(16));
        }

        /// <inheritdoc/>
        public CryptoModeEnum CryptoMode => CryptoModeEnum.AES;
    }
}
