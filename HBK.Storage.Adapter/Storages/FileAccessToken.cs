using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Storages
{
    /// <summary>
    /// 檔案存取權杖
    /// </summary>
    public partial class FileAccessToken : ITimeStampModel, ISoftDeleteModel
    {
        /// <summary>
        /// 取得或設定檔案存取權杖 ID
        /// </summary>
        [Key]
        [Column("FileAccessTokenID")]
        public Guid FileAccessTokenId { get; set; }
        /// <summary>
        /// 取得或設定檔案存取權杖流水號
        /// </summary>
        public long FileAccessTokenNo { get; set; }
        /// <summary>
        /// 取得或設定存取對象的儲存服務 ID
        /// </summary>
        [Column("StorageProviderID")]
        public Guid StorageProviderId { get; set; }
        /// <summary>
        /// 取得或設定強制指定的存取對象的儲存個體集合 ID
        /// </summary>
        [Column("StorageGroupID")]
        public Guid? StorageGroupId { get; set; }
        /// <summary>
        /// 取得或設定檔案實體 ID
        /// </summary>
        [Column("FileEntityID")]
        public Guid? FileEntityId { get; set; }
        /// <summary>
        /// 取得或設定權杖
        /// </summary>
        [Required]
        [StringLength(5000)]
        public string Token { get; set; }
        /// <summary>
        /// 取得或設定存取次數限制
        /// </summary>
        public int AccessTimesLimit { get; set; }
        /// <summary>
        /// 取得或設定存取次數
        /// </summary>
        public int AccessTimes { get; set; }
        /// <summary>
        /// 取得或設定過期時間
        /// </summary>
        public DateTimeOffset ExpireDateTime { get; set; }
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
        public FileAccessTokenStatusEnum Status { get; set; }

        /// <summary>
        /// 取得或設定檔案實體
        /// </summary>
        [ForeignKey(nameof(FileEntityId))]
        [InverseProperty("FileAccessToken")]
        public virtual FileEntity FileEntity { get; set; }
        /// <summary>
        /// 取得或設定儲存個體群組
        /// </summary>
        [ForeignKey(nameof(StorageGroupId))]
        [InverseProperty("FileAccessToken")]
        public virtual StorageGroup StorageGroup { get; set; }
        /// <summary>
        /// 取得或設定儲存服務
        /// </summary>
        [ForeignKey(nameof(StorageProviderId))]
        [InverseProperty("FileAccessToken")]
        public virtual StorageProvider StorageProvider { get; set; }

        DateTimeOffset? ITimeStampModel.UpdateDateTime { get => this.UpdateDateTime; set => this.UpdateDateTime = value; }
        DateTimeOffset ICreatedDateModel.CreateDateTime { get => this.CreateDateTime; set => this.CreateDateTime = value; }
        DateTimeOffset? ISoftDeleteModel.DeleteDateTime { get => this.DeleteDateTime; set => this.DeleteDateTime = value; }
    }
}
