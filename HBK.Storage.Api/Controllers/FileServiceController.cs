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

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="storageProviderService"></param>
        /// <param name="fileEntityService"></param>
        public FileServiceController(StorageProviderService storageProviderService, FileEntityService fileEntityService)
        {
            _storageProviderService = storageProviderService;
            _fileEntityService = fileEntityService;
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
    }
}
