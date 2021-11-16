using HBK.Storage.Api.FileAccessHandlers.Models;
using HBK.Storage.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 處理檔案實體的檔案存取處理器
    /// </summary>
    public class FileEntityAccessHandler : IFileAccessHandler
    {
        private readonly FileEntityService _fileEntityService;
        /// <summary>
        /// 初始化處理檔案實體的檔案存取處理器
        /// </summary>
        /// <param name="fileEntityService"></param>
        public FileEntityAccessHandler(FileEntityService fileEntityService)
        {
            _fileEntityService = fileEntityService;
        }
        /// <inheritdoc/>
        public async Task<FileAccessTaskModel> ProcessAsync(FileAccessTaskModel taskModel)
        {
            var fileEntityId = taskModel.Request.FileEntityId.HasValue ? taskModel.Request.FileEntityId.Value : taskModel.MiddleData.FileAccessToken.FileEntityId.Value;
            taskModel.MiddleData.FileEntity = await _fileEntityService.FindByIdAsync(fileEntityId);
            if (taskModel.MiddleData.FileEntity.Status.HasFlag(Adapter.Enums.FileEntityStatusEnum.Processing) ||
                taskModel.MiddleData.FileEntity.Status.HasFlag(Adapter.Enums.FileEntityStatusEnum.Uploading))
            {
                taskModel.ErrorObject = "檔案處理中";
            }

            return taskModel;
        }
    }
}
