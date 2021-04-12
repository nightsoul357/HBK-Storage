using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Models.FileService;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 檔案存取權帳控制器
    /// </summary>
    [Route("fileAccessToken")]
    public class FileAccessTokenController : HBKControllerBase
    {
        private readonly StorageProviderService _storageProviderService;
        private readonly FileEntityService _fileEntityService;
        private readonly FileAccessTokenService _fileAccessTokenService;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="storageProviderService"></param>
        /// <param name="fileEntityService"></param>
        /// <param name="fileAccessTokenService"></param>
        public FileAccessTokenController(StorageProviderService storageProviderService, FileEntityService fileEntityService, FileAccessTokenService fileAccessTokenService)
        {
            _storageProviderService = storageProviderService;
            _fileEntityService = fileEntityService;
            _fileAccessTokenService = fileAccessTokenService;
        }

        /// <summary>
        /// 產生檔案存取權杖
        /// </summary>
        /// <param name="storageProviderId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("storageProvider/{storageProviderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PostFileAccessTokenResponse>> PostFileAccessToken(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))]Guid storageProviderId,
            [FromBody] PostFileAccessTokenRequest request)
        {
            if ((request.FileAccessTokenType == FileAccessTokenTypeEnum.Normal || request.FileAccessTokenType == FileAccessTokenTypeEnum.NormalNoLimit) &&
                !request.FileEntityId.HasValue)
            {
                return base.BadRequest($"當 Token 類型為 { FileAccessTokenTypeEnum.Normal } 或 { FileAccessTokenTypeEnum.NormalNoLimit } 時，FileEntityId 不得為空");
            }
            if ((request.FileAccessTokenType == FileAccessTokenTypeEnum.Normal || request.FileAccessTokenType == FileAccessTokenTypeEnum.AllowTag) &&
                !request.AccessTimesLimit.HasValue)
            {
                return base.BadRequest($"當 Token 類型為 { FileAccessTokenTypeEnum.Normal } 或 { FileAccessTokenTypeEnum.AllowTag } 時，AccessTimesLimit 不得為空");
            }

            PostFileAccessTokenResponse result = new PostFileAccessTokenResponse();
            var expireDateTime = DateTime.Now.AddMinutes(request.ExpireAfterMinutes);
            result.ExpireDateTime = expireDateTime;

            switch (request.FileAccessTokenType)
            {
                case FileAccessTokenTypeEnum.Normal:
                    {
                        var token = await _fileAccessTokenService.GenerateNormalFileAccessTokenAsync(storageProviderId, request.StorageGroupId, request.FileEntityId.Value, expireDateTime, request.AccessTimesLimit.Value);
                        result.Token = token.Token;
                        result.UrlParrtenA = "/docs?esic={token}";
                        break;
                    }
                case FileAccessTokenTypeEnum.NormalNoLimit:
                    {
                        var token = _fileAccessTokenService.GenerateNormalNoLimitFileAccessToken(storageProviderId, request.StorageGroupId, request.FileEntityId.Value, expireDateTime);
                        result.Token = token;
                        result.UrlParrtenA = "/docs?esic={token}";
                        break;
                    }
                case FileAccessTokenTypeEnum.AllowTag:
                    {
                        var token = await _fileAccessTokenService.GenerateAllowTagFileAccessTokenAsync(storageProviderId, request.StorageGroupId, request.AllowTagPattern, expireDateTime, request.AccessTimesLimit.Value);
                        result.Token = token.Token;
                        result.UrlParrtenA = "/docs/{fileEntityId}?esic={token}";
                        break;
                    }
                case FileAccessTokenTypeEnum.AllowTagNoLimit:
                    {
                        var token = _fileAccessTokenService.GenerateAllowTagNoLimitFileAccessToken(storageProviderId, request.StorageGroupId, request.AllowTagPattern, expireDateTime);
                        result.Token = token;
                        result.UrlParrtenA = "/docs/{fileEntityId}?esic={token}";
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"尚未實作 { request.FileAccessTokenType } 的產生方法");
                    }
            }

            return result;
        }
    }
}
