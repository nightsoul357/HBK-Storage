using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.StorageCredentials;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.Models.FileService;
using HBK.Storage.Api.Models.Storage;
using HBK.Storage.Core.FileSystem.FTP;
using HBK.Storage.Core.Services;
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
    /// 儲存個體控制器
    /// </summary>
    [Route("storage")]
    public class StorageController : HBKControllerBase
    {
        private readonly ILogger<StorageController> _logger;
        private readonly StorageService _storageService;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="storageService"></param>
        /// <param name="logger"></param>
        public StorageController(StorageService storageService, ILogger<StorageController> logger)
        {
            _logger = logger;
            _storageService = storageService;
        }

        /// <summary>
        /// 取得指定 ID 之儲存個體
        /// </summary>
        /// <param name="storageId">儲存個體 ID</param>
        /// <returns></returns>
        [HttpGet("{storageId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<StorageResponse> GET(
            [ExampleParameter("00d89a53-107a-4666-ab46-03fc13fc9a93")]
            [ExistInDatabase(typeof(Adapter.Storages.Storage))] Guid storageId)
        {
            return StorageController.BuildStorageResponse(await _storageService.FindByIdAsync(storageId));
        }

        /// <summary>
        /// 產生儲存個體回應內容
        /// </summary>
        /// <param name="storage">儲存個體</param>
        /// <returns></returns>
        internal static StorageResponse BuildStorageResponse(Adapter.Storages.Storage storage)
        {
            return new StorageResponse()
            {
                CreateDateTime = storage.CreateDateTime.LocalDateTime,
                Credentials = storage.Credentials,
                Name = storage.Name,
                SizeLimit = storage.SizeLimit,
                Status = storage.Status.FlattenFlags(),
                StorageGroupId = storage.StorageGroupId,
                StorageId = storage.StorageId,
                Type = storage.Type,
                UpdateDateTime = storage.UpdateDateTime?.LocalDateTime
            };
        }
    }
}
