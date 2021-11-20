using HBK.Storage.Core.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileProcessHandlers
{
    /// <summary>
    /// 加密檔案處理器
    /// </summary>
    public class EncryptProcessHandler : DecryptProcessHandler
    {
        /// <summary>
        /// 初始化加密檔案處理器
        /// </summary>
        /// <param name="cryptoProviders"></param>
        public EncryptProcessHandler(IEnumerable<ICryptoProvider> cryptoProviders)
            : base(cryptoProviders)
        {
        }

        /// <inheritdoc/>
        public override string Name => "encrypt";
    }
}
