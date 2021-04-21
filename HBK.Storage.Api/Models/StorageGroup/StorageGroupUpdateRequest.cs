﻿using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.StorageGroup
{
    /// <summary>
    /// 更新儲存個體集合請求內容
    /// </summary>
    public class StorageGroupUpdateRequest
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
        /// 同步模式
        /// </summary>
        public SyncModeEnum SyncMode { get; set; }
        /// <summary>
        /// 同步策略
        /// </summary>
        public SyncPolicy SyncPolicy { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public StorageGroupStatusEnum[] Status { get; set; }
    }
}