using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 外部檔案存取控制器
    /// </summary>
    [AllowAnonymous]
    [Route("/docs")]
    public class ExternalFileAccessController : HBKControllerBase
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
        public ExternalFileAccessController(StorageProviderService storageProviderService, FileEntityService fileEntityService, FileAccessTokenService fileAccessTokenService)
        {
            _storageProviderService = storageProviderService;
            _fileEntityService = fileEntityService;
            _fileAccessTokenService = fileAccessTokenService;
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="esic">存取權杖</param>
        /// <returns></returns>
        [HttpGet("{fileEntityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DirectDownload(
            [ExistInDatabase(typeof(FileEntity))]
            [ExampleParameter("cfa83790-007c-4ba2-91b2-5b18dfe08735")]Guid fileEntityId,
            [FromQuery] string esic)
        {
            return await this.DoDirectForFileEntityIdDownloadAsync(fileEntityId, esic, string.Empty);
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="esic">存取權杖</param>
        /// <returns></returns>
        [HttpGet("{fileEntityId}/filename/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DirectDownload(
            [ExistInDatabase(typeof(FileEntity))]
            [ExampleParameter("cfa83790-007c-4ba2-91b2-5b18dfe08735")]Guid fileEntityId,
            [ExampleParameter("test.mp4")] string fileName,
            [FromQuery] string esic)
        {
            return await this.DoDirectForFileEntityIdDownloadAsync(fileEntityId, esic, fileName);
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="esic">存取權杖</param>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DirectDownload([FromQuery] string esic)
        {
            return await this.DoDirectDownloadAsync(esic, string.Empty);
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="esic">存取權杖</param>
        /// <param name="fileName">檔案名稱</param>
        /// <returns></returns>
        [HttpGet("filename/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> DirectDownload(
            [ExampleParameter("test.mp4")] string fileName,
            [FromQuery] string esic)
        {
            return await this.DoDirectDownloadAsync(esic, fileName);
        }

        private async Task<ActionResult> DoDirectDownloadAsync(string esic, string fileName)
        {
            if (string.IsNullOrWhiteSpace(esic))
            {
                return new NotFoundResult();
            }
            JwtSecurityToken jwtSecurityToken = await _fileAccessTokenService.ValidateFileAccessTokenAsync(esic);
            FileAccessTokenTypeEnum tokenType = Enum.Parse<FileAccessTokenTypeEnum>(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tokenType").Value);
            if (tokenType != FileAccessTokenTypeEnum.Normal && tokenType != FileAccessTokenTypeEnum.NormalNoLimit)
            {
                return base.BadRequest("使用的權杖類型無法透過此路徑存取檔案");
            }

            var fileAccessToken = _fileAccessTokenService.BuildFileAccessToken(jwtSecurityToken);
            var fileEntity = await _fileEntityService.FindByIdAsync(fileAccessToken.FileEntityId.Value);
            IAsyncFileInfo fileInfo = await _storageProviderService.GetAsyncFileInfoAsync(fileAccessToken.StorageProviderId, fileAccessToken.StorageGroupId, fileAccessToken.FileEntityId.Value);
            if (tokenType == FileAccessTokenTypeEnum.Normal)
            {
                await _fileAccessTokenService.TryAddAccessTimesAsync(fileAccessToken.FileAccessTokenId);
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = fileEntity.Name;
            }
            return new FileStreamResult(fileInfo.CreateReadStream(), new MediaTypeHeaderValue(fileEntity.MimeType))
            {
                FileDownloadName = fileName
            };
        }
        private async Task<ActionResult> DoDirectForFileEntityIdDownloadAsync(Guid fileEntityId, string esic, string fileName)
        {
            if (string.IsNullOrWhiteSpace(esic))
            {
                return new NotFoundResult();
            }
            JwtSecurityToken jwtSecurityToken = await _fileAccessTokenService.ValidateFileAccessTokenAsync(esic);
            FileAccessTokenTypeEnum tokenType = Enum.Parse<FileAccessTokenTypeEnum>(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tokenType").Value);
            if (tokenType != FileAccessTokenTypeEnum.AllowTag && tokenType != FileAccessTokenTypeEnum.AllowTagNoLimit)
            {
                return base.BadRequest("使用的權杖類型無法透過此路徑存取檔案");
            }

            var fileAccessToken = _fileAccessTokenService.BuildFileAccessToken(jwtSecurityToken);

            var fileEntity = await _fileEntityService.FindByIdAsync(fileEntityId);
            var allowTagPattern = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "allowTagPattern").Value;
            if (fileEntity.FileEntityTag.All(x => !Regex.IsMatch(x.Value, allowTagPattern)))
            {
                return base.BadRequest("沒有檔案存取權限");
            }

            IAsyncFileInfo fileInfo = await _storageProviderService.GetAsyncFileInfoAsync(fileAccessToken.StorageProviderId, fileAccessToken.StorageGroupId, fileEntityId);
            if (tokenType == FileAccessTokenTypeEnum.AllowTag)
            {
                await _fileAccessTokenService.TryAddAccessTimesAsync(fileAccessToken.FileAccessTokenId);
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = fileEntity.Name;
            }

            return new FileStreamResult(fileInfo.CreateReadStream(), new MediaTypeHeaderValue(fileEntity.MimeType))
            {
                FileDownloadName = fileName
            };
        }
    }
}
