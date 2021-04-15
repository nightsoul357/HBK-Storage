﻿using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.Models.FileEntity;
using HBK.Storage.Core.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 檔案實體控制器
    /// </summary>
    public class FileEntityController : HBKControllerBase
    {
        /// <summary>
        /// 產生檔案實體回應內容
        /// </summary>
        /// <param name="fileEntity">檔案實體</param>
        /// <param name="fileEntityService">檔案實體服務</param>
        /// <returns></returns>
        internal static FileEntityResponse BuildFileEntityResponse(FileEntity fileEntity, FileEntityService fileEntityService)
        {
            var storage = fileEntityService.GetStoragesAsync(fileEntity.FileEntityId).Result;
            return new FileEntityResponse()
            {
                CreateDateTime = fileEntity.CreateDateTime.LocalDateTime,
                ExtendProperty = fileEntity.ExtendProperty,
                FileEntityId = fileEntity.FileEntityId,
                MimeType = fileEntity.MimeType,
                Name = fileEntity.Name,
                Size = fileEntity.Size,
                Status = fileEntity.Status.FlattenFlags(),
                Tags = fileEntity.FileEntityTag.Select(x => x.Value).ToList(),
                UpdateDateTime = fileEntity.UpdateDateTime?.LocalDateTime,
                StorageSummaryResponses = storage.Select(x => StorageController.BuildStorageSummaryResponse(x)).ToList()
            };
        }
    }
}
