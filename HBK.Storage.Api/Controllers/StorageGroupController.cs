﻿using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Models;
using HBK.Storage.Api.Models.Storage;
using HBK.Storage.Api.Models.StorageGroup;
using HBK.Storage.Api.OData;
using HBK.Storage.Core.Services;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 儲存個體集合控制器
    /// </summary>
    [Route("storageGroup")]
    public class StorageGroupController : HBKControllerBase
    {
        private readonly StorageGroupService _storageGroupService;
        private readonly StorageService _storageService;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="storageGroupService"></param>
        /// <param name="storageService"></param>
        public StorageGroupController(StorageGroupService storageGroupService, StorageService storageService)
        {
            _storageGroupService = storageGroupService;
            _storageService = storageService;
        }

        /// <summary>
        /// 取得指定 ID 之儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <returns></returns>
        [HttpGet("{storageGroupId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageGroupResponse> Get(
            [ExampleParameter("8acdbf86-cb7b-4d1a-8745-44115f656287")]
            [ExistInDatabase(typeof(StorageGroup))]Guid storageGroupId)
        {
            var storageGroup = await _storageGroupService.FindByIdAsync(storageGroupId);
            return StorageGroupController.BuildStorageGroupResponse(storageGroup);
        }
        /// <summary>
        /// 新增儲存個體集合
        /// </summary>
        /// <param name="request">新增儲存個體集合請求內容</param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageGroupResponse> Post(StorageGroupAddRequest request)
        {
            var result = await _storageGroupService.AddAsync(new StorageGroup()
            {
                Name = request.Name,
                Status = request.Status.UnflattenFlags(),
                StorageProviderId = request.StorageProviderId,
                SyncMode = request.SyncMode,
                SyncPolicy = request.SyncPolicy,
                Type = request.Type
            });
            return StorageGroupController.BuildStorageGroupResponse(result);
        }

        /// <summary>
        /// 更新儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="request">更新儲存個體集合請求內容</param>
        /// <returns></returns>
        [HttpPut("{storageGroup}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageGroupResponse> Put(
            [ExampleParameter("8acdbf86-cb7b-4d1a-8745-44115f656287")]
            [ExistInDatabase(typeof(StorageGroup))] Guid storageGroupId,
            StorageGroupUpdateRequest request)
        {
            var storageGroup = await _storageGroupService.FindByIdAsync(storageGroupId);
            storageGroup.Name = request.Name;
            storageGroup.Status = request.Status.UnflattenFlags();
            storageGroup.SyncMode = request.SyncMode;
            storageGroup.SyncPolicy = request.SyncPolicy;
            storageGroup.Type = request.Type;
            var result = await _storageGroupService.UpdateAsync(storageGroup);
            return StorageGroupController.BuildStorageGroupResponse(storageGroup);
        }
        /// <summary>
        /// 刪除儲存個體集合(同時會刪除所有儲存個體)
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <returns></returns>
        [HttpDelete("{storageGroup}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(
            [ExampleParameter("8acdbf86-cb7b-4d1a-8745-44115f656287")]
            [ExistInDatabase(typeof(StorageGroup))] Guid storageGroupId)
        {
            await _storageGroupService.DeleteAsync(storageGroupId);
            return base.NoContent();
        }
        /// <summary>
        /// 取得儲存個體集合內的儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="queryOptions">OData 查詢選項</param>
        /// <returns></returns>
        [HttpGet("{storageGroupId}/storages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EnableODataQuery(AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.OrderBy,
            MaxTop = 100)]
        public async Task<PagedResponse<StorageResponse>> GetStorages(
            [ExampleParameter("8acdbf86-cb7b-4d1a-8745-44115f656287")]
            [ExistInDatabase(typeof(StorageGroup))] Guid storageGroupId,
            [FromServices] ODataQueryOptions<Adapter.Storages.Storage> queryOptions)
        {
            var query = _storageService.ListQuery()
                .Where(x => x.StorageGroupId == storageGroupId);

            return await base.PagedResultAsync(queryOptions, query, (data) =>
                data.Select(storage => StorageController.BuildStorageResponse(storage)),
                100
            );
        }

        /// <summary>
        /// 產生儲存個體集合回應內容
        /// </summary>
        /// <param name="storageGroup"></param>
        /// <returns></returns>
        internal static StorageGroupResponse BuildStorageGroupResponse(StorageGroup storageGroup)
        {
            return new StorageGroupResponse()
            {
                CreateDateTime = storageGroup.CreateDateTime,
                Name = storageGroup.Name,
                Status = storageGroup.Status.FlattenFlags(),
                StorageGroupId = storageGroup.StorageGroupId,
                StorageProviderId = storageGroup.StorageProviderId,
                SyncMode = storageGroup.SyncMode,
                SyncPolicy = storageGroup.SyncPolicy,
                Type = storageGroup.Type,
                UpdateDateTime = storageGroup.UpdateDateTime
            };
        }
    }
}