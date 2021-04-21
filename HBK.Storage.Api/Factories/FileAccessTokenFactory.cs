using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.Models.FileService;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Factories
{
    /// <summary>
    /// 產生檔案存取權杖的工廠
    /// </summary>
    public class FileAccessTokenFactory
    {
        private readonly FileAccessTokenService _fileAccessTokenService;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="fileAccessTokenService"></param>
        public FileAccessTokenFactory(FileAccessTokenService fileAccessTokenService)
        {
            _fileAccessTokenService = fileAccessTokenService;
        }
        /// <summary>
        /// 取得檔案存取權杖
        /// </summary>
        /// <param name="storageProviderId"></param>
        /// <param name="expireDateTime"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> GetFileAccessTokenAsync(Guid storageProviderId, DateTime expireDateTime, PostFileAccessTokenRequest request)
        {
            switch (request.FileAccessTokenType)
            {
                case FileAccessTokenTypeEnum.Normal:
                    {
                        return (await _fileAccessTokenService
                            .GenerateNormalFileAccessTokenAsync(storageProviderId, request.StorageGroupId, request.FileEntityId.Value, expireDateTime, request.AccessTimesLimit.Value)).Token;
                    }
                case FileAccessTokenTypeEnum.NormalNoLimit:
                    {
                        return _fileAccessTokenService
                                .GenerateNormalNoLimitFileAccessToken(storageProviderId, request.StorageGroupId, request.FileEntityId.Value, expireDateTime);
                    }
                case FileAccessTokenTypeEnum.AllowTag:
                    {
                        return (await _fileAccessTokenService
                            .GenerateAllowTagFileAccessTokenAsync(storageProviderId, request.StorageGroupId, request.AllowTagPattern, expireDateTime, request.AccessTimesLimit.Value)).Token;
                    }
                case FileAccessTokenTypeEnum.AllowTagNoLimit:
                    {
                        return _fileAccessTokenService
                            .GenerateAllowTagNoLimitFileAccessToken(storageProviderId, request.StorageGroupId, request.AllowTagPattern, expireDateTime);
                    }
                default:
                    {
                        throw new NotImplementedException($"尚未實作 { request.FileAccessTokenType } 的產生方法");
                    }
            }
        }
    }
}
