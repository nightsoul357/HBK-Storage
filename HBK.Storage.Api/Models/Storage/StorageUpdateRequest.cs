using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.StorageCredentials;
using HBK.Storage.Api.JsonConverters;
using HBK.Storage.Api.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.Storage
{
    /// <summary>
    /// 更新儲存個體請求內容
    /// </summary>
    [ModelBinder(BinderType = typeof(StorageCredentialsModelBinder))]
    public class StorageUpdateRequest
    {
        /// <summary>
        /// 名稱
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public StorageTypeEnum Type { get; set; }
        /// <summary>
        /// 檔案大小限制(單位 Bytes)
        /// </summary>
        public long SizeLimit { get; set; }
        /// <summary>
        /// 儲存個體的驗證資訊
        /// </summary>
        [JsonConverter(typeof(StorageCredentialsJsonConverter))]
        public StorageCredentialsBase Credentials { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public StorageStatusEnum[] Status { get; set; }
    }
}
