using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Models.FileAccessToken;
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
    [Route("fileAccessTokens")]
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
        /// 取得指定 ID 之檔案存取權杖
        /// </summary>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <returns></returns>
        [HttpGet("{fileAccessTokenId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<FileAccessTokenResponse> Get(
            [ExampleParameter("651961b0-c12c-46af-9cc5-d7997a7c94fb")]
            [ExistInDatabase(typeof(FileAccessToken))] Guid fileAccessTokenId)
        {
            var fileAccessToken = await _fileAccessTokenService.FindByIdAsync(fileAccessTokenId);
            return FileAccessTokenController.BuildFileAccessTokenResponse(fileAccessToken);
        }
        /// <summary>
        /// 撤銷指定 ID 的權杖
        /// </summary>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <returns></returns>
        [HttpPut("{fileAccessTokenId}/revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<FileAccessTokenResponse> Revoke(
            [ExampleParameter("651961b0-c12c-46af-9cc5-d7997a7c94fb")]
            [ExistInDatabase(typeof(FileAccessToken))] Guid fileAccessTokenId)
        {
            var fileAccessToken = await _fileAccessTokenService.RevokeTokenAsync(fileAccessTokenId);
            return FileAccessTokenController.BuildFileAccessTokenResponse(fileAccessToken);
        }

        /// <summary>
        /// 產生檔案存取權杖回應內容
        /// </summary>
        /// <param name="fileAccessToken"></param>
        /// <returns></returns>
        internal static FileAccessTokenResponse BuildFileAccessTokenResponse(FileAccessToken fileAccessToken)
        {
            return new FileAccessTokenResponse()
            {
                AccessTimes = fileAccessToken.AccessTimes,
                AccessTimesLimit = fileAccessToken.AccessTimesLimit,
                CreateDateTime = fileAccessToken.CreateDateTime,
                ExpireDateTime = fileAccessToken.ExpireDateTime,
                FileAccessTokenId = fileAccessToken.FileAccessTokenId,
                FileEntityId = fileAccessToken.FileEntityId,
                Status = fileAccessToken.Status.FlattenFlags(),
                StorageGroupId = fileAccessToken.StorageGroupId,
                StorageProviderId = fileAccessToken.StorageProviderId,
                Token = fileAccessToken.Token,
                UpdateDateTime = fileAccessToken.UpdateDateTime
            };
        }
    }
}