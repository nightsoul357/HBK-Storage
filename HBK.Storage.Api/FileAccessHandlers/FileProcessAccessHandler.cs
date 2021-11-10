using HBK.Storage.Api.FileAccessHandlers.Models;
using HBK.Storage.Api.FileProcessHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 處理檔案處理器代理執行者檔案存取處理器
    /// </summary>
    public class FileProcessAccessHandler : IFileAccessHandler
    {
        private readonly FileProcessHandlerProxy _fileProcessHandlerProxy;

        /// <summary>
        /// 初始化處理檔案處理器代理執行者檔案存取處理器
        /// </summary>
        /// <param name="fileProcessHandlerProxy"></param>
        public FileProcessAccessHandler(FileProcessHandlerProxy fileProcessHandlerProxy)
        {
            _fileProcessHandlerProxy = fileProcessHandlerProxy;
        }

        /// <inheritdoc/>
        public async Task<FileAccessTaskModel> ProcessAsync(FileAccessTaskModel taskModel)
        {
            var processResult = await _fileProcessHandlerProxy.ProcessAsync(new FileProcessTaskModel()
            {
                FileEntity = taskModel.MiddleData.FileEntity,
                FileInfo = taskModel.MiddleData.FileInfo,
                Token = taskModel.MiddleData.JwtSecurityToken,
                StorageProviderId = taskModel.MiddleData.FileAccessToken.StorageProviderId
            }, taskModel.MiddleData.JwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "handlerIndicate").Value);

            taskModel.MiddleData.FileProcessTaskModel = processResult;
            return taskModel;
        }
    }
}
