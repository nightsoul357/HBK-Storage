using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Models;
using HBK.Storage.Api.Models.StorageProvider;
using HBK.Storage.Api.OData;
using HBK.Storage.Core.Services;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 儲存服務控制器
    /// </summary>
    [Route("storageProvider")]
    public class StorageProviderController : HBKControllerBase
    {
        private readonly ILogger<StorageProviderController> _logger;
        private readonly StorageProviderService _storageProviderService;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="storageProviderService"></param>
        /// <param name="logger"></param>
        public StorageProviderController(StorageProviderService storageProviderService, ILogger<StorageProviderController> logger)
        {
            _logger = logger;
            _storageProviderService = storageProviderService;
        }

        /// <summary>
        /// 取得指定 ID 之儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <returns></returns>
        [HttpGet("{storageProviderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageProviderResponse> GET(
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

            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(storageProvider => StorageProviderController.BuildStorageProviderResponse(storageProvider)),
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
