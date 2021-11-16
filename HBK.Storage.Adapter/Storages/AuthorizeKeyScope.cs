using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HBK.Storage.Adapter.Storages
{
    /// <summary>
    /// 金鑰使用範圍資訊
    /// </summary>
    public partial class AuthorizeKeyScope : ITimeStampModel
    {
        /// <summary>
        /// 取得或設定流水號
        /// </summary>
        [Key]
        public long AuthorizeKeyScopeNo { get; set; }
        /// <summary>
        /// 取得或設定金鑰 ID
        /// </summary>
        [Column("AuthorizeKeyID")]
        public Guid AuthorizeKeyId { get; set; }
        /// <summary>
        /// 取得或設定儲存服務 ID
        /// </summary>
        [Column("StorageProviderID")]
        public Guid StorageProviderId { get; set; }
        /// <summary>
        /// 取得建立時間
        /// </summary>
        public DateTimeOffset CreateDateTime { get; set; }
        /// <summary>
        /// 取得最後更新時間
        /// </summary>
        public DateTimeOffset? UpdateDateTime { get; set; }
        /// <summary>
        /// 取得或設定允許的操作類型
        /// </summary>
        public AuthorizeKeyScopeOperationTypeEnum AllowOperationType { get; set; }

        /// <summary>
        /// 取得或設定金鑰
        /// </summary>
        [ForeignKey(nameof(AuthorizeKeyId))]
        [InverseProperty("AuthorizeKeyScope")]
        public virtual AuthorizeKey AuthorizeKey { get; set; }
        /// <summary>
        /// 取得或設定儲存服務
        /// </summary>
        [ForeignKey(nameof(StorageProviderId))]
        [InverseProperty("AuthorizeKeyScope")]
        public virtual StorageProvider StorageProvider { get; set; }

        DateTimeOffset? ITimeStampModel.UpdateDateTime { get => this.UpdateDateTime; set => this.UpdateDateTime = value; }
        DateTimeOffset ICreatedDateModel.CreateDateTime { get => this.CreateDateTime; set => this.CreateDateTime = value; }
    }
}
