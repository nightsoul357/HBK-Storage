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
    /// 驗證用的金鑰資訊
    /// </summary>
    public partial class AuthorizeKey : ITimeStampModel, ISoftDeleteModel
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public AuthorizeKey()
        {
            this.AuthorizeKeyScope = new HashSet<AuthorizeKeyScope>();
        }

        /// <summary>
        /// 取得或設定金鑰 ID
        /// </summary>
        [Key]
        [Column("AuthorizeKeyID")]
        public Guid AuthorizeKeyId { get; set; }
        /// <summary>
        /// 取得或設定流水號
        /// </summary>
        public long AuthorizeKeyNo { get; set; }
        /// <summary>
        /// 取得或設定金鑰值
        /// </summary>
        [Required]
        [StringLength(4000)]
        public string KeyValue { get; set; }
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
        /// 取得或設定類型
        /// </summary>
        public AuthorizeKeyTypeEnum Type { get; set; }
        /// <summary>
        /// 取得或設定額為訊息
        /// </summary>
        [StringLength(4000)]
        public string ExtendProperty { get; set; }
        /// <summary>
        /// 取得或設定狀態
        /// </summary>
        public AuthorizeKeyStatusEnum Status { get; set; }

        /// <summary>
        /// 取得或設定金鑰使用範圍集合
        /// </summary>
        [InverseProperty("AuthorizeKey")]
        public virtual ICollection<AuthorizeKeyScope> AuthorizeKeyScope { get; set; }

        DateTimeOffset? ITimeStampModel.UpdateDateTime { get => this.UpdateDateTime; set => this.UpdateDateTime = value; }
        DateTimeOffset ICreatedDateModel.CreateDateTime { get => this.CreateDateTime; set => this.CreateDateTime = value; }
        DateTimeOffset? ISoftDeleteModel.DeleteDateTime { get => this.DeleteDateTime; set => this.DeleteDateTime = value; }
    }
}
