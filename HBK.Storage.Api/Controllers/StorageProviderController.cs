using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Factories;
using HBK.Storage.Api.Filters;
using HBK.Storage.Api.Models;
using HBK.Storage.Api.Models.FileAccessToken;
using HBK.Storage.Api.Models.FileEntity;
using HBK.Storage.Api.Models.FileService;
using HBK.Storage.Api.Models.StorageGroup;
using HBK.Storage.Api.Models.StorageProvider;
using HBK.Storage.Api.OData;
using HBK.Storage.Core.Cryptography;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.Models;
using HBK.Storage.Core.Services;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 儲存服務控制器
    /// </summary>
    [Route("storageProviders")]
    public class StorageProviderController : HBKControllerBase
    {
        private readonly ILogger<StorageProviderController> _logger;
        private readonly StorageProviderService _storageProviderService;
        private readonly StorageGroupService _storageGroupService;
        private readonly FileEntityService _fileEntityService;
        private readonly FileAccessTokenService _fileAccessTokenService;
        private readonly FileAccessTokenFactory _fileAccessTokenFactory;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="storageProviderService"></param>
        /// <param name="storageGroupService"></param>
        /// <param name="fileEntityService"></param>
        /// <param name="fileAccessTokenService"></param>
        /// <param name="fileAccessTokenFactory"></param>
        /// <param name="logger"></param>
        public StorageProviderController(StorageProviderService storageProviderService, StorageGroupService storageGroupService, FileEntityService fileEntityService, FileAccessTokenService fileAccessTokenService, FileAccessTokenFactory fileAccessTokenFactory, ILogger<StorageProviderController> logger)
        {
            _logger = logger;
            _storageProviderService = storageProviderService;
            _storageGroupService = storageGroupService;
            _fileEntityService = fileEntityService;
            _fileAccessTokenService = fileAccessTokenService;
            _fileAccessTokenFactory = fileAccessTokenFactory;
        }
        /// <summary>
        /// 取得指定 ID 之儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <returns></returns>
        [HttpGet("{storageProviderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageProviderResponse> Get(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))] Guid storageProviderId)
        {
            return StorageProviderController.BuildStorageProviderResponse(await _storageProviderService.FindByIdAsync(storageProviderId));
        }
        /// <summary>
        /// 取得所有儲存服務資訊，單次資料上限為 100 筆
        /// </summary>
        /// <param name="queryOptions">OData 查詢參數</param>
        /// <returns>儲存服務資訊集合</returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableODataQuery(AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.OrderBy,
            MaxTop = 100)]
        public async Task<ActionResult<PagedResponse<StorageProviderResponse>>> List([FromServices] ODataQueryOptions<StorageProvider> queryOptions)
        {
            var query = _storageProviderService.ListQuery();

            if (base.AuthorizeKey.Value.Type == AuthorizeKeyTypeEnum.Normal)
            {
                var authorizeKeyId = base.AuthorizeKey.Value.AuthorizeKeyId;
                query = query
                    .Where(storageProvider => storageProvider.AuthorizeKeyScope
                        .Any(scope => scope.AuthorizeKeyId == authorizeKeyId && scope.AllowOperationType == AuthorizeKeyScopeOperationTypeEnum.Read));
            }

            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(storageProvider => StorageProviderController.BuildStorageProviderResponse(storageProvider)),
                100
            );
        }
        /// <summary>
        /// 加入儲存服務
        /// </summary>
        /// <param name="request">加入儲存服務請求內容</param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<StorageProviderResponse> Post([FromBody] StorageProviderAddRequest request)
        {
            var sotrageProvider = await _storageProviderService.AddAsync(new StorageProvider()
            {
                Name = request.Name,
                Status = request.Status.UnflattenFlags()
            });
            return StorageProviderController.BuildStorageProviderResponse(sotrageProvider);
        }
        /// <summary>
        /// 更新儲存服務內容
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="request">更新儲存服務內容請求內容</param>
        /// <returns></returns>
        [HttpPut("{storageProviderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageProviderResponse> Put(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))] Guid storageProviderId,
            [FromBody] StorageProviderUpdateRequest request)
        {
            var storageProvider = await _storageProviderService.FindByIdAsync(storageProviderId);
            storageProvider.Name = request.Name;
            storageProvider.Status = request.Status.UnflattenFlags();
            var result = await _storageProviderService.UpdateAsync(storageProvider);
            return StorageProviderController.BuildStorageProviderResponse(result);
        }
        /// <summary>
        /// 刪除儲存服務(包含所有儲存個體集合)
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <returns></returns>
        [HttpDelete("{storageProviderId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))] Guid storageProviderId)
        {
            await _storageProviderService.DeleteAsync(storageProviderId);
            return base.NoContent();
        }

        /// <summary>
        /// 新增儲存個體集合
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="request">新增儲存個體集合請求內容</param>
        /// <returns></returns>
        [HttpPost("{storageProviderId}/storageGroups")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageGroupResponse> Post(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))] Guid storageProviderId,
            StorageGroupAddRequest request)
        {
            var result = await _storageGroupService.AddAsync(new StorageGroup()
            {
                Name = request.Name,
                Status = request.Status.UnflattenFlags(),
                StorageProviderId = storageProviderId,
                SyncMode = request.SyncMode,
                SyncPolicy = new Adapter.Models.SyncPolicy()
                {
                    Rule = request.SyncPolicy.Rule
                },
                ClearMode = request.ClearMode,
                ClearPolicy = new Adapter.Models.ClearPolicy()
                {
                    Rule = request.ClearPolicy.Rule
                },
                UploadPriority = request.UploadPriority,
                DownloadPriority = request.DownloadPriority,
                Type = request.Type
            });
            return StorageGroupController.BuildStorageGroupResponse(result);
        }
        /// <summary>
        /// 取得儲存服務內的所有儲存群組集合，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="queryOptions">OData 查詢選項</param>
        /// <returns></returns>
        [HttpGet("{storageProviderId}/storageGroups")]
        [EnableODataQuery(AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.OrderBy,
            MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<PagedResponse<StorageGroupResponse>> GetStorageGroups(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))] Guid storageProviderId,
            [FromServices] ODataQueryOptions<StorageGroup> queryOptions)
        {
            var query = _storageGroupService.ListQuery()
                .Where(x => x.StorageProviderId == storageProviderId);

            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(storgaeGroup => StorageGroupController.BuildStorageGroupResponse(storgaeGroup)),
                100
            );
        }
        /// <summary>
        /// 取得儲存服務內的所有儲存群組擴充資訊集合，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="queryOptions">OData 查詢選項</param>
        /// <returns></returns>
        [HttpGet("{storageProviderId}/storageGroupExtendProperties")]
        [EnableODataQuery(AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.OrderBy,
            MaxTop = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<PagedResponse<StorageGroupExtendPropertyResponse>> GetStorageGroupExtendProperties(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))] Guid storageProviderId,
            [FromServices] ODataQueryOptions<StorageGroupExtendProperty> queryOptions)
        {
            var query = _storageGroupService.GetStorageGroupExtendPropertiesQuery()
               .Where(x => x.StorageGroup.StorageProviderId == storageProviderId);

            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(storageGroupExtendProperty => StorageGroupController.BuildStorageGroupExtendPropertyResponse(storageGroupExtendProperty)),
                100
            );
        }
        /// <summary>
        /// 取得儲存服務內的檔案實體集合，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="queryOptions">OData 查詢參數</param>
        /// <returns></returns>
        [HttpGet("{storageProviderId}/fileEntities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableODataQuery(AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.OrderBy,
            MaxTop = 100)]
        public async Task<PagedResponse<FileEntityResponse>> GetFileEntityResponses(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))] Guid storageProviderId,
            [FromServices] ODataQueryOptions<FileEntity> queryOptions)
        {
            var query = _fileEntityService.ListQuery()
                .Where(x => x.FileEntityStroage.Any(f => f.Storage.StorageGroup.StorageProviderId == storageProviderId));

            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(fileEntity => FileEntityController.BuildFileEntityResponse(fileEntity, _fileEntityService)),
                100
            );
        }
        /// <summary>
        /// 上傳檔案至指定的儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="request">上傳檔案的請求內容</param>
        /// <returns></returns>
        [HttpPost("{storageProviderId}/fileEntities")]
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
                        FileEntityTag = request.Tags.Select(x => new FileEntityTag() { Value = x }).ToList(),
                        AccessType = AccessTypeEnum.Private,
                        CryptoMode = request.CryptoMode
                    },
                    request.FileStream, "Uploda Service");
            return FileEntityController.BuildFileEntityResponse(fileEntity, _fileEntityService);
        }

        /// <summary>
        /// 產生檔案存取權杖
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{storageProviderId}/fileAccessTokens")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            result.Token = await _fileAccessTokenFactory.GetFileAccessTokenAsync(storageProviderId, expireDateTime, request);
            return result;
        }
        /// <summary>
        /// 取得檔案存取權杖清單，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="queryOptions">OData 查詢參數</param>
        /// <returns></returns>
        [HttpGet("/{storageProviderId}/fileAccessTokens")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableODataQuery(AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.OrderBy,
            MaxTop = 100)]
        public async Task<ActionResult<PagedResponse<FileAccessTokenResponse>>> GetFileAccessTokens(
            [ExampleParameter("59b50410-e86a-4341-8973-ae325e354210")]
            [ExistInDatabase(typeof(StorageProvider))]Guid storageProviderId,
            [FromServices] ODataQueryOptions<FileAccessToken> queryOptions)
        {
            var query = _fileAccessTokenService.ListQuery()
                .Where(x => x.StorageProviderId == storageProviderId);

            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(fileAccessToke => FileAccessTokenController.BuildFileAccessTokenResponse(fileAccessToke)),
                100
            );
        }
        /// <summary>
        /// 產生儲存服務回應內容
        /// </summary>
        /// <param name="storageProvider">儲存服務</param>
        /// <returns>儲存服務回應內容</returns>
        internal static StorageProviderResponse BuildStorageProviderResponse(StorageProvider storageProvider)
        {
            return new StorageProviderResponse()
            {
                CreateDateTime = storageProvider.CreateDateTime.LocalDateTime,
                Name = storageProvider.Name,
                Status = storageProvider.Status.FlattenFlags(),
                StorageProviderId = storageProvider.StorageProviderId,
                UpdateDateTime = storageProvider.UpdateDateTime?.LocalDateTime
            };
        }
    }
}