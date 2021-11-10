using HBK.Storage.Api.FileAccessHandlers.Models;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 取得檔案資訊的檔案存取處理器
    /// </summary>
    public class FetchFileInfoAccessHandler : IFileAccessHandler
    {
        private readonly FileEntityService _fileEntityService;
        private readonly StorageProviderService _storageProviderService;
        private readonly FileEntityStorageService _fileEntityStorageService;
        /// <summary>
        /// 初始化取得檔案資訊的檔案存取處理器
        /// </summary>
        /// <param name="storageProviderService"></param>
        /// <param name="fileEntityStorageService"></param>
        /// <param name="fileEntityService"></param>
        public FetchFileInfoAccessHandler(StorageProviderService storageProviderService, FileEntityStorageService fileEntityStorageService, FileEntityService fileEntityService)
        {
            _storageProviderService = storageProviderService;
            _fileEntityStorageService = fileEntityStorageService;
            _fileEntityService = fileEntityService;
        }

        /// <inheritdoc/>
        public async Task<FileAccessTaskModel> ProcessAsync(FileAccessTaskModel taskModel)
        {
            Adapter.Storages.FileEntityStorage fileEntityStorage;
            if (taskModel.MiddleData.FileAccessToken == null)
            {
                var storageProviderId = await _fileEntityService.GetStorageProviderIdByFileEntityIdAsync(taskModel.MiddleData.FileEntity.FileEntityId);
                fileEntityStorage = await _storageProviderService.GetFileEntityStorageAsync(storageProviderId, null, taskModel.MiddleData.FileEntity.FileEntityId);
            }
            else
            {
                fileEntityStorage = await _storageProviderService.GetFileEntityStorageAsync(taskModel.MiddleData.FileAccessToken.StorageProviderId, taskModel.MiddleData.FileAccessToken.StorageGroupId, taskModel.MiddleData.FileEntity.FileEntityId);
            }

            IAsyncFileInfo fileInfo = await _fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId);
            taskModel.MiddleData.FileInfo = fileInfo;
            if (fileInfo == null)
            {
                taskModel.ErrorObject = "找不到有效的檔案";
            }
            return taskModel;
        }
    }
}
