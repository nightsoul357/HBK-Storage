using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Filters;
using HBK.Storage.Api.Helpers;
using HBK.Storage.Api.Models.FileEntity;
using HBK.Storage.Api.Models.FileService;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 檔案服務提供者控制器
    /// </summary>
    [Route("fileService")]
    public class FileServiceController : HBKControllerBase
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
        public FileServiceController(StorageProviderService storageProviderService, FileEntityService fileEntityService, FileAccessTokenService fileAccessTokenService)
        {
            _storageProviderService = storageProviderService;
            _fileEntityService = fileEntityService;
            _fileAccessTokenService = fileAccessTokenService;
        }

        /// <summary>
        /// 上傳檔案至指定的儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="request">上傳檔案的請求內容</param>
        /// <returns></returns>
        [HttpPut("storageProvider/{storageProviderId}")]
        [DisableRequestSizeLimit]
        [DisableFormValueModelBindingFilter]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<FileEntityResponse> Put(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))]Guid storageProviderId,
            [FromForm] PutFileRequest request)
        {
            var fileEntity = await _storageProviderService
                .UploadFileEntityAsync(storageProviderId,
                    request.StorageGroupId,
                    new FileEntity()
                    {
                        ExtendProperty = request.ExtendProperty,
                        MimeType = request.MimeType,
                        Name = request.Filename,
                        Size = base.HttpContext.Request.ContentLength.Value,
                        Status = FileEntityStatusEnum.None,
                        FileEntityTag = request.Tags.Select(x => new FileEntityTag() { Value = x }).ToList()
                    },
                    request.FileStream);
            return FileEntityController.BuildFileEntityResponse(fileEntity);
        }

        /// <summary>
        /// 產生檔案存取權杖
        /// </summary>
        /// <param name="storageProviderId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("storageProvider/{storageProviderId}/token")]
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
                        result.UrlParrtenA = $"/docs/{request.FileEntityId.Value}?esic=" + "{token}";
                        result.UrlParrtenB = "/docs?esic={token}";
                        break;
                    }
                case FileAccessTokenTypeEnum.NormalNoLimit:
                    {
                        var token = _fileAccessTokenService.GenerateNormalNoLimitFileAccessToken(storageProviderId, request.StorageGroupId, request.FileEntityId.Value, expireDateTime);
                        result.Token = token;
                        result.UrlParrtenA = $"/docs/{request.FileEntityId.Value}?esic="+"{token}";
                        result.UrlParrtenB = "/docs?esic={token}";
                        break;
                    }
                case FileAccessTokenTypeEnum.AllowTag:
                    {
                        var token = await _fileAccessTokenService.GenerateAllowTagFileAccessTokenAsync(storageProviderId, request.StorageGroupId, request.AllowTagPattern, expireDateTime, request.AccessTimesLimit.Value);
                        result.Token = token.Token;
                        result.UrlParrtenA = $"/docs/{request.FileEntityId.Value}?esic=" + "{token}";
                        break;
                    }
                case FileAccessTokenTypeEnum.AllowTagNoLimit:
                    {
                        var token = _fileAccessTokenService.GenerateAllowTagNoLimitFileAccessToken(storageProviderId, request.StorageGroupId, request.AllowTagPattern, expireDateTime);
                        result.Token = token;
                        result.UrlParrtenA = $"/docs/{request.FileEntityId.Value}?esic=" + "{token}";
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException($"尚未實作 { request.FileAccessTokenType } 的產生方法");
                    }
            }

            return result;
        }

        /// <summary>
        /// 從指定的儲存服務下載檔案
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="storageGroupId">強制指定儲存個體群組 ID</param>
        /// <returns></returns>
        [HttpGet("storageProvider/{storageProviderId}/fileEntity/{fileEntityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<FileStreamResult> Get(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))]Guid storageProviderId,
            [ExampleParameter("ba337d2f-760b-473e-b077-d352277651e2")]
            [ExistInDatabase(typeof(FileEntity))]Guid fileEntityId,
            [FromQuery] Guid? storageGroupId)
        {
            var fileEntity = await _fileEntityService.FindByIdAsync(fileEntityId);
            var fileInfo = await _storageProviderService.GetAsyncFileInfoAsync(storageProviderId, storageGroupId, fileEntityId);

            return new FileStreamResult(fileInfo.CreateReadStream(), new MediaTypeHeaderValue(fileEntity.MimeType))
            {
                FileDownloadName = fileEntity.Name
            };
        }

        /// <summary>
        /// 將檔案實體標記為刪除
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns></returns>
        [HttpDelete("fileEntity/{fileEntityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(
            [ExistInDatabase(typeof(FileEntity))]
            [ExampleParameter("cfa83790-007c-4ba2-91b2-5b18dfe08735")]Guid fileEntityId)
        {
            await _fileEntityService.MarkFileEntityDeleteAsync(fileEntityId);
            return base.NoContent();
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="esic">存取權杖</param>
        /// <returns></returns>
        [HttpGet("/docs/{fileEntityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
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
        [HttpGet("/docs/{fileEntityId}/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
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
        [HttpGet("/docs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
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
        [HttpGet("/docs/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
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
            FileAccessTokenTypeEnum tokenType = (FileAccessTokenTypeEnum)Convert.ToInt32(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tokenType").Value);
            if (tokenType != FileAccessTokenTypeEnum.Normal || tokenType != FileAccessTokenTypeEnum.NormalNoLimit)
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

            FileAccessTokenTypeEnum tokenType = (FileAccessTokenTypeEnum)Convert.ToInt32(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tokenType").Value);
            if (tokenType != FileAccessTokenTypeEnum.AllowTag || tokenType != FileAccessTokenTypeEnum.AllowTagNoLimit)
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
