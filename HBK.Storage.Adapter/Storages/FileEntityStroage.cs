﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Interfaces;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace HBK.Storage.Adapter.Storages
{
    /// <summary>
    /// 檔案位於儲存個體上的橋接資訊
    /// </summary>
    public partial class FileEntityStroage : ITimeStampModel, ISoftDeleteModel
    {
        /// <summary>
        /// 設定或取得檔案位於儲存個體上的橋接資訊 ID
        /// </summary>
        [Key]
        [Column("FileEntityStroageID")]
        public Guid FileEntityStroageId { get; set; }
        /// <summary>
        /// 設定或取得檔案位於儲存個體上的橋接資訊流水號
        /// </summary>
        public long FileEntityStroageNo { get; set; }
        /// <summary>
        /// 設定或取得檔案實體 ID
        /// </summary>
        [Column("FileEntityID")]
        public Guid FileEntityId { get; set; }
        /// <summary>
        /// 設定或取得儲存個體 ID
        /// </summary>
        [Column("StorageID")]
        public Guid StorageId { get; set; }
        /// <summary>
        /// 設定或取得存取檔案識別值
        /// </summary>
        [Required]
        [StringLength(1023)]
        public string Value { get; set; }
        /// <summary>
        /// 設定或取得建立者識別名稱
        /// </summary>
        [Required]
        [StringLength(255)]
        public string CreatorIdentity { get; set; }
        /// <summary>
        /// 取得或設定是否標記刪除
        /// </summary>
        public bool IsMarkDelete { get; set; }
        /// <summary>
        /// 取得建立時間
        /// </summary>
        public DateTimeOffset CreateDateTime { get; internal set; }
        /// <summary>
        /// 取得最後更新時間
        /// </summary>
        public DateTimeOffset? UpdateDateTime { get; internal set; }
        /// <summary>
        /// 取得刪除時間時間
        /// </summary>
        public DateTimeOffset? DeleteDateTime { get; internal set; }
        /// <summary>
        /// 設定或取得狀態
        /// </summary>
        public FileEntityStorageStatusEnum Status { get; set; }

        /// <summary>
        /// 設定或取得對應的檔案實體
        /// </summary>
        [ForeignKey(nameof(FileEntityId))]
        [InverseProperty("FileEntityStroage")]
        public virtual FileEntity FileEntity { get; set; }
        /// <summary>
        /// 設定或取得對應的儲存個體
        /// </summary>
        [ForeignKey(nameof(StorageId))]
        [InverseProperty("FileEntityStroage")]
        public virtual Storage Storage { get; set; }
        DateTimeOffset? ITimeStampModel.UpdateDateTime { get => this.UpdateDateTime; set => this.UpdateDateTime = value; }
        DateTimeOffset ICreatedDateModel.CreateDateTime { get => this.CreateDateTime; set => this.CreateDateTime = value; }
        DateTimeOffset? ISoftDeleteModel.DeleteDateTime { get => this.DeleteDateTime; set => this.DeleteDateTime = value; }
    }
}