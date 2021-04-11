using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Filters;
using HBK.Storage.Api.Helpers;
using HBK.Storage.Api.Models.FileEntity;
using HBK.Storage.Api.Models.FileService;
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
        /// 產生檔案直接下載位置
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="request">產生檔案直接下載位置請求內容</param>
        /// <returns></returns>
        [HttpPost("storageProvider/{storageProviderId}/fileEntity/{fileEntityId}/direct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<GetExternalDirectLinkResponse> PostExternalDirectLink(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))]Guid storageProviderId,
            [ExampleParameter("ba337d2f-760b-473e-b077-d352277651e2")]
            [ExistInDatabase(typeof(FileEntity))]Guid fileEntityId,
            [FromBody] PostExternalDirectLinkRequest request)
        {
            var fileAccessToken = await _fileAccessTokenService.GenerateFileAccessTokenAsync(new FileAccessToken()
            {
                AccessTimesLimit = request.AccessTimesLimit,
                FileEntityId = fileEntityId,
                StorageGroupId = request.StorageGroupId,
                StorageProviderId = storageProviderId,
                ExpireDateTime = DateTime.Now.AddMinutes(request.ExpireAfterMinutes)
            });

            string url = $"{base.Request.Scheme}://{base.Request.Host}/docs?esic={fileAccessToken.Token}";
            return new GetExternalDirectLinkResponse()
            {
                ExpireDateTime = fileAccessToken.ExpireDateTime.LocalDateTime,
                Url = url
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
        /// <param name="esic">存取權杖</param>
        /// <returns></returns>
        [HttpGet("/docs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult> DirectDownload([FromQuery] string esic)
        {
            if (string.IsNullOrWhiteSpace(esic))
            {
                return new NotFoundResult();
            }

            var jwtSecurityToken = _fileAccessTokenService.ValidateFileAccessToken(esic);
            var id = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var fileAccessToken = await _fileAccessTokenService.FindByIdAsync(Guid.Parse(id));
            if (fileAccessToken.Status.HasFlag(FileAccessTokenStatusEnum.Revoke))
            {
                return new NotFoundResult();
            }
            if (fileAccessToken.AccessTimesLimit <= fileAccessToken.AccessTimes)
            {
                return new NotFoundResult();
            }

            await _fileAccessTokenService.TryAddAccessTimesAsync(fileAccessToken.FileAccessTokenId);
            var fileEntity = await _fileEntityService.FindByIdAsync(fileAccessToken.FileEntityId);
            var fileInfo = await _storageProviderService.GetAsyncFileInfoAsync(fileAccessToken.StorageProviderId, fileAccessToken.StorageGroupId, fileAccessToken.FileEntityId);

            return new FileStreamResult(fileInfo.CreateReadStream(), new MediaTypeHeaderValue(fileEntity.MimeType))
            {
                FileDownloadName = fileEntity.Name
            };
        }
    }
}
