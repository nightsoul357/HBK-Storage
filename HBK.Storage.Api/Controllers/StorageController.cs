using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.StorageCredentials;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.ModelBinders;
using HBK.Storage.Api.Models.FileService;
using HBK.Storage.Api.Models.Storage;
using HBK.Storage.Core.FileSystem.FTP;
using HBK.Storage.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 儲存個體控制器
    /// </summary>
    [Route("storages")]
    public class StorageController : HBKControllerBase
    {
        private readonly ILogger<StorageController> _logger;
        private readonly StorageService _storageService;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="storageService"></param>
        /// <param name="logger"></param>
        /// <param name="jsonOptions"></param>
        public StorageController(StorageService storageService, ILogger<StorageController> logger, IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
        {
            _logger = logger;
            _storageService = storageService;
            _jsonSerializerSettings = jsonOptions.Value.SerializerSettings;
        }

        /// <summary>
        /// 取得指定 ID 之儲存個體
        /// </summary>
        /// <param name="storageId">儲存個體 ID</param>
        /// <returns></returns>
        [HttpGet("{storageId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageResponse> Get(
            [ExampleParameter("00d89a53-107a-4666-ab46-03fc13fc9a93")]
            [ExistInDatabase(typeof(Adapter.Storages.Storage))] Guid storageId)
        {
            return StorageController.BuildStorageResponse(await _storageService.FindByIdAsync(storageId), _jsonSerializerSettings);
        }

        /// <summary>
        /// 更新儲存個體
        /// </summary>
        /// <param name="storageId">儲存個體 ID</param>
        /// <param name="request">更新儲存個體請求內容</param>
        /// <returns></returns>
        [HttpPut("{storageId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageResponse> Put(
            [ExampleParameter("00d89a53-107a-4666-ab46-03fc13fc9a93")]
            [ExistInDatabase(typeof(Adapter.Storages.Storage))] Guid storageId,
            [FromBody] StorageUpdateRequest request)
        {
            var storage = await _storageService.FindByIdAsync(storageId);
            storage.Credentials = request.Credentials;
            storage.Name = request.Name;
            storage.SizeLimit = request.SizeLimit;
            storage.Status = request.Status.UnflattenFlags();
            storage.Type = request.Type;
            var result = await _storageService.UpdateAsync(storage);
            return StorageController.BuildStorageResponse(result, _jsonSerializerSettings);
        }
        /// <summary>
        /// 刪除儲存個體(同時刪除儲存個體內的所有檔案)
        /// </summary>
        /// <param name="storageId">儲存個體 ID</param>
        /// <returns></returns>
        [HttpDelete("{storageId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(
            [ExampleParameter("00d89a53-107a-4666-ab46-03fc13fc9a93")]
            [ExistInDatabase(typeof(Adapter.Storages.Storage))] Guid storageId)
        {
            await _storageService.DeleteAsync(storageId);
            return base.NoContent();
        }
        /// <summary>
        /// 產生儲存個體回應內容
        /// </summary>
        /// <param name="storage">儲存個體</param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        internal static StorageResponse BuildStorageResponse(Adapter.Storages.Storage storage, JsonSerializerSettings jsonSerializerSettings)
        {
            return new StorageResponse()
            {
                CreateDateTime = storage.CreateDateTime.LocalDateTime,
                Credentials = JsonConvert.SerializeObject(storage.Credentials, jsonSerializerSettings),
                Name = storage.Name,
                SizeLimit = storage.SizeLimit,
                Status = storage.Status.FlattenFlags(),
                StorageId = storage.StorageId,
                Type = storage.Type,
                UpdateDateTime = storage.UpdateDateTime?.LocalDateTime,
                StorageGroupResponse = StorageGroupController.BuildStorageGroupResponse(storage.StorageGroup)
            };
        }
        /// <summary>
        /// 產生儲存個體基本資訊回應內容
        /// </summary>
        /// <param name="storage">儲存個體</param>
        /// <returns></returns>
        internal static StorageSummaryResponse BuildStorageSummaryResponse(Adapter.Storages.Storage storage)
        {
            return new StorageSummaryResponse()
            {
                CreateDateTime = storage.CreateDateTime.LocalDateTime,
                Name = storage.Name,
                SizeLimit = storage.SizeLimit,
                Status = storage.Status.FlattenFlags(),
                StorageId = storage.StorageId,
                Type = storage.Type,
                UpdateDateTime = storage.UpdateDateTime?.LocalDateTime,
                StorageGroupResponse = StorageGroupController.BuildStorageGroupResponse(storage.StorageGroup)
            };
        }
    }
}
