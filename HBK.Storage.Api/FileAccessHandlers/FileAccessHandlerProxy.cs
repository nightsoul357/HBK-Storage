using HBK.Storage.Api.FileAccessHandlers.Models;
using HBK.Storage.Api.FileProcessHandlers;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 檔案存取處理器代理執行者
    /// </summary>
    public class FileAccessHandlerProxy
    {
        private FileAccessTokenService _fileAccessTokenService;
        private readonly FileProcessHandlerProxy _fileProcessHandlerProxy;
        private readonly FileEntityService _fileEntityService;
        private readonly StorageProviderService _storageProviderService;
        private readonly FileEntityStorageService _fileEntityStorageService;
        /// <summary>
        /// 初始化檔案存取處理器代理執行者
        /// </summary>
        /// <param name="fileAccessTokenService"></param>
        /// <param name="fileProcessHandlerProxy"></param>
        /// <param name="fileEntityService"></param>
        /// <param name="storageProviderService"></param>
        /// <param name="fileEntityStorageService"></param>
        public FileAccessHandlerProxy(FileAccessTokenService fileAccessTokenService, FileProcessHandlerProxy fileProcessHandlerProxy, FileEntityService fileEntityService, StorageProviderService storageProviderService, FileEntityStorageService fileEntityStorageService)
        {
            _fileAccessTokenService = fileAccessTokenService;
            _fileProcessHandlerProxy = fileProcessHandlerProxy;
            _fileEntityService = fileEntityService;
            _storageProviderService = storageProviderService;
            _fileEntityStorageService = fileEntityStorageService;
        }
        /// <summary>
        /// 執行一般下載
        /// </summary>
        /// <param name="esic"></param>
        /// <returns></returns>
        public async Task<FileAccessTaskModel> DoDirectDownloadAsync(string esic)
        {
            List<IFileAccessHandler> fileAccessHandlers = new List<IFileAccessHandler>()
            {
                new JwtSecurityTokenAccessHandler(_fileAccessTokenService,FileAccessTokenTypeEnum.Normal,FileAccessTokenTypeEnum.NormalNoLimit),
                new FileEntityAccessHandler(_fileEntityService),
                new FetchFileInfoAccessHandler(_storageProviderService,_fileEntityStorageService,_fileEntityService),
                new AddTokenAccessTimesAccessHandler(_fileAccessTokenService),
                new FileProcessAccessHandler(_fileProcessHandlerProxy)
            };
            var result = await this.ExecuteFileAccessHandlerAsync(fileAccessHandlers, new FileAccessTaskModel(esic));
            if (result.ErrorObject != null)
            {
                return result;
            }

            result.FileInfo = result.MiddleData.FileProcessTaskModel.FileInfo;
            return result;
        }
        /// <summary>
        /// 執行對於指定檔案實體 ID 之下載
        /// </summary>
        /// <param name="fileEntityId"></param>
        /// <param name="esic"></param>
        /// <returns></returns>
        public async Task<FileAccessTaskModel> DoDirectForFileEntityIdDownloadAsync(Guid fileEntityId, string esic)
        {
            List<IFileAccessHandler> fileAccessHandlers = new List<IFileAccessHandler>()
            {
                new JwtSecurityTokenAccessHandler(_fileAccessTokenService,FileAccessTokenTypeEnum.AllowTag,FileAccessTokenTypeEnum.AllowTagNoLimit),
                new FileEntityAccessHandler(_fileEntityService),
                new ValidateTagPatternAccessHandler(),
                new FetchFileInfoAccessHandler(_storageProviderService,_fileEntityStorageService,_fileEntityService),
                new AddTokenAccessTimesAccessHandler(_fileAccessTokenService),
                new FileProcessAccessHandler(_fileProcessHandlerProxy)
            };
            var result = await this.ExecuteFileAccessHandlerAsync(fileAccessHandlers, new FileAccessTaskModel(fileEntityId, esic));
            if (result.ErrorObject != null)
            {
                return result;
            }

            result.FileInfo = result.MiddleData.FileProcessTaskModel.FileInfo;
            return result;
        }

        /// <summary>
        /// 下載公開的檔案
        /// </summary>
        /// <param name="fileEntityId"></param>
        /// <returns></returns>
        public async Task<FileAccessTaskModel> DoDirectForPublicFileEntityDownloadAsync(Guid fileEntityId)
        {
            List<IFileAccessHandler> fileAccessHandlers = new List<IFileAccessHandler>()
            {
                new FileEntityAccessHandler(_fileEntityService),
                new ValidateFileEntityAccessTypeAccessHandler(Adapter.Enums.AccessTypeEnum.Public),
                new FetchFileInfoAccessHandler(_storageProviderService,_fileEntityStorageService,_fileEntityService),
            };

            var result = await this.ExecuteFileAccessHandlerAsync(fileAccessHandlers, new FileAccessTaskModel(fileEntityId));
            if (result.ErrorObject != null)
            {
                return result;
            }

            result.FileInfo = result.MiddleData.FileInfo;
            return result;
        }

        private async Task<FileAccessTaskModel> ExecuteFileAccessHandlerAsync(List<IFileAccessHandler> fileAccessHandlers, FileAccessTaskModel taskModel)
        {
            foreach (var handler in fileAccessHandlers)
            {
                taskModel = await handler.ProcessAsync(taskModel);
                if (taskModel.ErrorObject != null)
                {
                    return taskModel;
                }
            }
            return taskModel;
        }
    }
}
