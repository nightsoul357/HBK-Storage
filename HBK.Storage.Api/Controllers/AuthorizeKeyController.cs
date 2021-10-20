using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.Comparers;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Filters;
using HBK.Storage.Api.Models;
using HBK.Storage.Api.Models.AuthorizeKey;
using HBK.Storage.Api.OData;
using HBK.Storage.Core.Services;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 驗證金鑰控制器
    /// </summary>
    [AllowRoot]
    [Route("authorizeKey")]
    public class AuthorizeKeyController : HBKControllerBase
    {
        private readonly AuthorizeKeyService _authorizeKeyService;
        /// <summary>
        /// 建立新的執行個體
        /// </summary>
        /// <param name="authorizeKeyService"></param>
        public AuthorizeKeyController(AuthorizeKeyService authorizeKeyService)
        {
            _authorizeKeyService = authorizeKeyService;
        }
        /// <summary>
        /// 取得指定 ID 驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId"></param>
        /// <returns></returns>
        [HttpGet("{authorizeKeyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<AuthorizeKeyResponse> Get(
            [ExampleParameter("81be2e36-fa66-46ea-b7a7-f5848a7f79b2")]
            [ExistInDatabase(typeof(AuthorizeKey))]Guid authorizeKeyId)
        {
            var authorizeKey = await _authorizeKeyService.FindByIdAsync(authorizeKeyId);
            return AuthorizeKeyController.BuildAuthorizeKeyResponse(authorizeKey);
        }
        /// <summary>
        /// 取得指定金鑰值的驗證金鑰
        /// </summary>
        /// <param name="keyValue">金鑰值</param>
        /// <returns></returns>
        [HttpGet("key/{keyValue}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizeKeyResponse>> GetByKeyValue(string keyValue)
        {
            var authorizeKey = await _authorizeKeyService.FindByKeyValueAsync(keyValue);
            if (authorizeKey == null)
            {
                return base.NotFound();
            }
            return AuthorizeKeyController.BuildAuthorizeKeyResponse(authorizeKey);
        }
        /// <summary>
        /// 新增驗證金鑰
        /// </summary>
        /// <param name="request">新增驗證金鑰請求內容</param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthorizeKeyResponse>> Post(AddAuthorizeKeyRequest request)
        {
            request.AddAuthorizeKeyScopeRequests = request.AddAuthorizeKeyScopeRequests.Distinct(AddAuthorizeKeyScopeRequestComparer.GetInstance()).ToList();

            var result = await _authorizeKeyService.AddAsync(new Adapter.Storages.AuthorizeKey()
            {
                AuthorizeKeyScope = request.AddAuthorizeKeyScopeRequests.Select(x => new AuthorizeKeyScope()
                {
                    AllowOperationType = x.AllowOperationType,
                    StorageProviderId = x.StorageProviderId
                }).ToList(),
                Name = request.Name,
                Type = request.Type
            });
            return AuthorizeKeyController.BuildAuthorizeKeyResponse(result, false); // 僅在新增時，允許回傳 Key Value
        }
        /// <summary>
        /// 停用驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns></returns>
        [HttpPut("{authorizeKeyId}/disable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizeKeyResponse>> DisableKey(
            [ExampleParameter("81be2e36-fa66-46ea-b7a7-f5848a7f79b2")]
            [ExistInDatabase(typeof(AuthorizeKey))]Guid authorizeKeyId)
        {
            var result = await _authorizeKeyService.DiableAsync(authorizeKeyId);
            return AuthorizeKeyController.BuildAuthorizeKeyResponse(result);
        }
        /// <summary>
        /// 啟用驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns></returns>
        [HttpPut("{authorizeKeyId}/enable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorizeKeyResponse>> EnableKey(
            [ExampleParameter("81be2e36-fa66-46ea-b7a7-f5848a7f79b2")]
            [ExistInDatabase(typeof(AuthorizeKey))]Guid authorizeKeyId)
        {
            var result = await _authorizeKeyService.EnableAsync(authorizeKeyId);
            return AuthorizeKeyController.BuildAuthorizeKeyResponse(result);
        }
        /// <summary>
        /// 刪除驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns></returns>
        [HttpDelete("{authorizeKeyId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(
            [ExampleParameter("81be2e36-fa66-46ea-b7a7-f5848a7f79b2")]
            [ExistInDatabase(typeof(AuthorizeKey))]Guid authorizeKeyId)
        {
            await _authorizeKeyService.DeleteAsync(authorizeKeyId);
            return base.NotFound();
        }
        /// <summary>
        /// 取得所有驗證金鑰，單次資料上限為 100 筆
        /// </summary>
        /// <param name="queryOptions">OData 查詢參數</param>
        /// <returns>驗證金鑰資訊集合</returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableODataQuery(AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.OrderBy,
            MaxTop = 100)]
        public async Task<ActionResult<PagedResponse<AuthorizeKeyResponse>>> List([FromServices] ODataQueryOptions<AuthorizeKey> queryOptions)
        {
            var query = _authorizeKeyService.ListQuery();

            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(authorizeKey => AuthorizeKeyController.BuildAuthorizeKeyResponse(authorizeKey)),
                100
            );
        }

        /// <summary>
        /// 產生驗證金鑰回應內容
        /// </summary>
        /// <param name="authorizeKey">驗證金鑰</param>
        /// <param name="isMaskKey">是否遮蔽金鑰值</param>
        /// <returns></returns>
        internal static AuthorizeKeyResponse BuildAuthorizeKeyResponse(AuthorizeKey authorizeKey, bool isMaskKey = true)
        {
            return new AuthorizeKeyResponse()
            {
                AuthorizeKeyId = authorizeKey.AuthorizeKeyId,
                AuthorizeKeyScopeResponses = authorizeKey.AuthorizeKeyScope.Select(x => new AuthorizeKeyScopeResponse()
                {
                    AllowOperationType = x.AllowOperationType,
                    CreateDateTime = x.CreateDateTime,
                    StorageProviderId = x.StorageProviderId,
                    UpdateDateTime = x.UpdateDateTime
                }).ToList(),
                CreateDateTime = authorizeKey.CreateDateTime,
                KeyValue = isMaskKey ? string.Empty : authorizeKey.KeyValue,
                Name = authorizeKey.Name,
                Status = authorizeKey.Status.FlattenFlags(),
                Type = authorizeKey.Type,
                UpdateDateTime = authorizeKey.UpdateDateTime
            };
        }
    }
}
