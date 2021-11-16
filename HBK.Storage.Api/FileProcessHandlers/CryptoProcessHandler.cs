using HBK.Storage.Core.Cryptography;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.FileSystem.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileProcessHandlers
{
    /// <summary>
    /// 處理加密後的檔案處理器
    /// </summary>
    public class CryptoProcessHandler : FileProcessHandlerBase
    {
        private readonly IEnumerable<ICryptoProvider> _cryptoProviders;
        /// <summary>
        /// 初始化處理加密後的檔案處理器
        /// </summary>
        /// <param name="cryptoProviders"></param>
        public CryptoProcessHandler(IEnumerable<ICryptoProvider> cryptoProviders)
        {
            _cryptoProviders = cryptoProviders;
        }
        /// <inheritdoc/>
        public override Task<FileProcessTaskModel> ProcessAsync(FileProcessTaskModel taskModel)
        {
            if (taskModel.FileEntity.CryptoMode != Adapter.Enums.CryptoModeEnum.NoCrypto)
            {
                var cryptoProvider = _cryptoProviders.FirstOrDefault(x => x.CryptoMode == taskModel.FileEntity.CryptoMode);
                var fileInfo = new StreamFileInfo(
                    new CryptoStream(taskModel.FileInfo.CreateReadStream(),
                        cryptoProvider,
                        taskModel.FileEntity.CryptoKey,
                        taskModel.FileEntity.CryptoIv),
                    taskModel.FileInfo.Length);

                taskModel.FileInfo = fileInfo;
            }
            return Task.FromResult(taskModel);
        }

        /// <inheritdoc/>
        public override string Name => "decrypt";
    }
}
