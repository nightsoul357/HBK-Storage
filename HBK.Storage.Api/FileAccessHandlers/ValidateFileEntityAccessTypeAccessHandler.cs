using HBK.Storage.Adapter.Enums;
using HBK.Storage.Api.FileAccessHandlers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 驗證檔案存取類型的檔案存取處理器
    /// </summary>
    public class ValidateFileEntityAccessTypeAccessHandler : IFileAccessHandler
    {
        private AccessTypeEnum _accessType;
        /// <summary>
        /// 初始化驗證檔案存取類型的檔案存取處理器
        /// </summary>
        /// <param name="accessType"></param>
        public ValidateFileEntityAccessTypeAccessHandler(AccessTypeEnum accessType)
        {
            _accessType = accessType;
        }

        /// <inheritdoc/>
        public Task<FileAccessTaskModel> ProcessAsync(FileAccessTaskModel taskModel)
        {
            if (taskModel.MiddleData.FileEntity.AccessType != _accessType)
            {
                taskModel.ErrorObject = "沒有檔案存取權限";
            }
            return Task.FromResult(taskModel);
        }
    }
}
