using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Models;
using HBK.Storage.Api.Models.FileAccessToken;
using HBK.Storage.Api.Models.FileEntity;
using HBK.Storage.Api.Models.FileService;
using HBK.Storage.Api.OData;
using HBK.Storage.Core.Models;
using HBK.Storage.Core.Services;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 檔案實體控制器
    /// </summary>
    [Route("fileEntities")]
    public class FileEntityController : HBKControllerBase
    {
        private readonly StorageProviderService _storageProviderService;
        private readonly FileEntityService _fileEntityService;
        private readonly FileEntityStorageService _fileEntityStorageService;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="storageProviderService"></param>
        /// <param name="fileEntityService"></param>
        /// <param name="fileEntityStorageService"></param>
        public FileEntityController(StorageProviderService storageProviderService, FileEntityService fileEntityService, FileEntityStorageService fileEntityStorageService)
        {
            _storageProviderService = storageProviderService;
            _fileEntityService = fileEntityService;
            _fileEntityStorageService = fileEntityStorageService;
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="storageGroupId">強制指定儲存個體群組 ID</param>
        /// <returns></returns>
        [HttpGet("{fileEntityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [FileStreamResultResponse]
        public async Task<FileStreamResult> Get(
            [ExampleParameter("ba337d2f-760b-473e-b077-d352277651e2")]
            [ExistInDatabase(typeof(FileEntity))]Guid fileEntityId,
            [FromQuery] Guid? storageGroupId)
        {
            var storageProviderId = await _fileEntityService.GetStorageProviderIdByFileEntityIdAsync(fileEntityId);
            var fileEntity = await _fileEntityService.FindByIdAsync(fileEntityId);
            var fileEntityStorage = await _storageProviderService.GetFileEntityStorageAsync(storageProviderId, storageGroupId, fileEntityId);
            var fileInfo = await _fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStorage.FileEntityStorageId);

            return new FileStreamResult(fileInfo.CreateReadStream(), new MediaTypeHeaderValue(fileEntity.MimeType))
            {
                FileDownloadName = fileEntity.Name
            };
        }
        /// <summary>
        /// 更新檔案實體
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="request">更新檔案實體請求內容</param>
        /// <returns></returns>
        [HttpPut("{fileEntityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<FileEntityResponse> Put(
            [ExampleParameter("ba337d2f-760b-473e-b077-d352277651e2")]
            [ExistInDatabase(typeof(FileEntity))]Guid fileEntityId,
            FileEntityUpdateRequest request)
        {
            var fileEntity = await _fileEntityService.FindByIdAsync(fileEntityId);
            fileEntity.Name = request.Filename;
            fileEntity.ExtendProperty = request.ExtendProperty;
            fileEntity.AccessType = request.AccessType;
            fileEntity = await _fileEntityService.UpdateAsync(fileEntity);
            return FileEntityController.BuildFileEntityResponse(fileEntity, _fileEntityService);
        }
        /// <summary>
        /// 取得檔案的所有存取權杖資訊，單次資料上限為 100 筆
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="queryOptions">OData 查詢參數</param>
        /// <returns></returns>
        [HttpGet("{fileEntityId}/fileAccessTokens")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<PagedResponse<FileAccessTokenResponse>> GetFileAccessTokenResponse(
            [ExampleParameter("ba337d2f-760b-473e-b077-d352277651e2")]
            [ExistInDatabase(typeof(FileEntity))]Guid fileEntityId,
            [FromServices] ODataQueryOptions<FileAccessToken> queryOptions)
        {
            var query = _fileEntityService.GetFileAccessTokenQuery(fileEntityId);

            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(fileAccessToken => FileAccessTokenController.BuildFileAccessTokenResponse(fileAccessToken)),
                100
            );
        }
        /// <summary>
        /// 取得指定檔案的所有子檔案(遞迴查詢)
        /// </summary>
        /// <param name="fileEntityId">檔案 ID</param>
        /// <param name="queryOptions">查詢參數</param>
        /// <returns></returns>
        [HttpGet("{fileEntityId}/childs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableODataQuery(AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.OrderBy,
            MaxTop = 100)]
        public async Task<PagedResponse<ChildFileEntityResponse>> GetChildFileEntities(
            [ExampleParameter("ba337d2f-760b-473e-b077-d352277651e2")]
            [ExistInDatabase(typeof(FileEntity))]Guid fileEntityId,
            [FromServices] ODataQueryOptions<ChildFileEntity> queryOptions)
        {
            var query = _fileEntityService.GetChildFileEntitiesQuery(fileEntityId);
            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(childFileEntity => FileEntityController.BuildChildFileEntityResponse(childFileEntity)),
                100
            );
        }
        /// <summary>
        /// 附加檔案實體標籤
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="request">附加檔案實體標籤請求內容</param>
        /// <returns></returns>
        [HttpPost("{fileEntityId}/tag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<FileEntityResponse> AppendTag(
            [ExampleParameter("ba337d2f-760b-473e-b077-d352277651e2")]
            [ExistInDatabase(typeof(FileEntity))] Guid fileEntityId,
            AppendTagRequest request)
        {
            await _fileEntityService.AppendTagAsync(fileEntityId, request.Tag);
            return FileEntityController.BuildFileEntityResponse(await _fileEntityService.FindByIdAsync(fileEntityId), _fileEntityService);
        }

        /// <summary>
        /// 刪除檔案實體標籤
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="request">刪除檔案實體標籤請求內容</param>
        /// <returns></returns>
        [HttpDelete("{fileEntityId}/tag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<FileEntityResponse> DeleteTag(
            [ExampleParameter("ba337d2f-760b-473e-b077-d352277651e2")]
            [ExistInDatabase(typeof(FileEntity))] Guid fileEntityId,
            DeleteTagRequest request)
        {
            await _fileEntityService.RemoveTagAsync(fileEntityId, request.Tag);
            return FileEntityController.BuildFileEntityResponse(await _fileEntityService.FindByIdAsync(fileEntityId), _fileEntityService);
        }

        /// <summary>
        /// 將檔案實體標記為刪除
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns></returns>
        [HttpDelete("{fileEntityId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(
            [ExistInDatabase(typeof(FileEntity))]
            [ExampleParameter("cfa83790-007c-4ba2-91b2-5b18dfe08735")]Guid fileEntityId)
        {
            await _fileEntityService.MarkFileEntityDeleteAsync(fileEntityId);
            return base.NoContent();
        }

        /// <summary>
        /// 取得檔案實體存取次數
        /// </summary>
        /// <param name="fileEntityId"></param>
        /// <returns></returns>
        [HttpGet("{fileEntityId}/access_times")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<GetAccessTimesResponse> GetAccessTimes(
            [ExistInDatabase(typeof(FileEntity))]
            [ExampleParameter("cfa83790-007c-4ba2-91b2-5b18dfe08735")]Guid fileEntityId)
        {
            return new GetAccessTimesResponse()
            {
                TotalAccessTimes = await _fileEntityService.GetAccessTimesAsync(fileEntityId)
            };
        }

        /// <summary>
        /// 產生檔案實體回應內容
        /// </summary>
        /// <param name="fileEntity">檔案實體</param>
        /// <param name="fileEntityService">檔案實體服務</param>
        /// <returns></returns>
        internal static FileEntityResponse BuildFileEntityResponse(FileEntity fileEntity, FileEntityService fileEntityService)
        {
            var fileEntityStorages = fileEntityService.GetFileEntityStroageAsync(fileEntity.FileEntityId).Result;
            return new FileEntityResponse()
            {
                CreateDateTime = fileEntity.CreateDateTime.LocalDateTime,
                ExtendProperty = fileEntity.ExtendProperty,
                FileEntityId = fileEntity.FileEntityId,
                MimeType = fileEntity.MimeType,
                AccessType = fileEntity.AccessType,
                Name = fileEntity.Name,
                Size = fileEntity.Size,
                Status = fileEntity.Status.FlattenFlags(),
                Tags = fileEntity.FileEntityTag.Select(x => x.Value).ToList(),
                UpdateDateTime = fileEntity.UpdateDateTime?.LocalDateTime,
                FileEntityStorageResponses = fileEntityStorages.Select(x => FileEntityController.BuildFileEntityStorageResponse(x)).ToList(),
            };
        }
        /// <summary>
        /// 產生檔案實體於儲存個體上的回應內容
        /// </summary>
        /// <param name="fileEntityStorage">檔案位於儲存個體上的橋接資訊</param>
        /// <returns></returns>
        internal static FileEntityStorageResponse BuildFileEntityStorageResponse(FileEntityStorage fileEntityStorage)
        {
            return new FileEntityStorageResponse()
            {
                CreateDateTime = fileEntityStorage.Storage.CreateDateTime.LocalDateTime,
                FileEntityStorageStatus = fileEntityStorage.Status.FlattenFlags(),
                Status = fileEntityStorage.Storage.Status.FlattenFlags(),
                Name = fileEntityStorage.Storage.Name,
                SizeLimit = fileEntityStorage.Storage.SizeLimit,
                StorageId = fileEntityStorage.Storage.StorageId,
                Type = fileEntityStorage.Storage.Type,
                UpdateDateTime = fileEntityStorage.Storage.UpdateDateTime?.LocalDateTime,
                CreatorIdentity = fileEntityStorage.CreatorIdentity,
                FileEntityStorageCreateDateTime = fileEntityStorage.CreateDateTime.LocalDateTime,
                FileEntityStorageUpdateDateTime = fileEntityStorage.UpdateDateTime?.LocalDateTime,
                StorageGroupResponse = StorageGroupController.BuildStorageGroupResponse(fileEntityStorage.Storage.StorageGroup)
            };
        }
        /// <summary>
        /// 產生 Child 的檔案實體回應內容
        /// </summary>
        /// <param name="childFileEntity">Child 的檔案實體</param>
        /// <returns></returns>
        internal static ChildFileEntityResponse BuildChildFileEntityResponse(ChildFileEntity childFileEntity)
        {
            return new ChildFileEntityResponse()
            {
                CreateDateTime = childFileEntity.FileEntity.CreateDateTime.LocalDateTime,
                ExtendProperty = childFileEntity.FileEntity.ExtendProperty,
                FileEntityId = childFileEntity.FileEntity.FileEntityId,
                MimeType = childFileEntity.FileEntity.MimeType,
                Name = childFileEntity.FileEntity.Name,
                Size = childFileEntity.FileEntity.Size,
                Status = childFileEntity.FileEntity.Status.FlattenFlags(),
                Tags = childFileEntity.FileEntity.FileEntityTag.Select(x => x.Value).ToList(),
                UpdateDateTime = childFileEntity.FileEntity.UpdateDateTime?.LocalDateTime,
                ChildLevel = childFileEntity.ChildLevel,
                RootFileEntityId = childFileEntity.RootFileEntityId
            };
        }
    }
}
