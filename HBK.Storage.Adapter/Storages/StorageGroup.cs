﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Interfaces;
using HBK.Storage.Adapter.Models;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HBK.Storage.Adapter.Storages
{
    /// <summary>
    /// 儲存個體群組
    /// </summary>
    public partial class StorageGroup : ITimeStampModel, ISoftDeleteModel
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public StorageGroup()
        {
            this.Storage = new HashSet<Storage>();
            this.FileAccessToken = new HashSet<FileAccessToken>();
        }
        /// <summary>
        /// 取得或設定儲存個體群組 ID
        /// </summary>
        [Key]
        [Column("StorageGroupID")]
        public Guid StorageGroupId { get; set; }
        /// <summary>
        /// 取得或設定儲存個體群組流水號
        /// </summary>
        public long StorageGroupNo { get; set; }
        /// <summary>
        /// 取得或設定所屬儲存服務 ID
        /// </summary>
        [Column("StorageProviderID")]
        public Guid? StorageProviderId { get; set; }
        /// <summary>
        /// 取得或設定名稱
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        /// <summary>
        /// 取得或設定類型
        /// </summary>
        public StorageTypeEnum Type { get; set; }
        /// <summary>
        /// 取得或設定同步模式
        /// </summary>
        public SyncModeEnum SyncMode { get; set; }
        /// <summary>
        /// 取得或設定同步策略
        /// </summary>
        [StringLength(511)]
        public SyncPolicy SyncPolicy { get; set; }
        /// <summary>
        /// 取得或設定上傳優先權
        /// </summary>
        public int UploadPriority { get; set; }
        /// <summary>
        /// 取得或設定下載優先權
        /// </summary>
        public int DownloadPriority { get; set; }
        /// <summary>
        /// 取得建立時間
        /// </summary>
        public DateTimeOffset CreateDateTime { get; internal set; }
        /// <summary>
        /// 取得最後更新時間
        /// </summary>
        public DateTimeOffset? UpdateDateTime { get; internal set; }
        /// <summary>
        /// 取得刪除時間
        /// </summary>
        public DateTimeOffset? DeleteDateTime { get; internal set; }
        /// <summary>
        /// 取得或設定狀態
        /// </summary>
        public StorageGroupStatusEnum Status { get; set; }

        /// <summary>
        /// 取得或設定所屬儲存服務
        /// </summary>
        [ForeignKey(nameof(StorageProviderId))]
        [InverseProperty("StorageGroup")]
        public virtual StorageProvider StorageProvider { get; set; }
        /// <summary>
        /// 取得或設定擁有的儲存個體群組
        /// </summary>
        [InverseProperty("StorageGroup")]
        public virtual ICollection<Storage> Storage { get; set; }
        /// <summary>
        /// 取得或設定檔案存取權杖集合
        /// </summary>
        [InverseProperty("StorageGroup")]
        public virtual ICollection<FileAccessToken> FileAccessToken { get; set; }
        DateTimeOffset? ITimeStampModel.UpdateDateTime { get => this.UpdateDateTime; set => this.UpdateDateTime = value; }
        DateTimeOffset ICreatedDateModel.CreateDateTime { get => this.CreateDateTime; set => this.CreateDateTime = value; }
        DateTimeOffset? ISoftDeleteModel.DeleteDateTime { get => this.DeleteDateTime; set => this.DeleteDateTime = value; }
    }
}