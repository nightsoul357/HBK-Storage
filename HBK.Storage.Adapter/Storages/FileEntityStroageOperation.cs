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
    /// 檔案位於儲存個體上的操作紀錄
    /// </summary>
    public class FileEntityStroageOperation : ITimeStampModel
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public FileEntityStroageOperation()
        {
            this.ExtendProperty = string.Empty; // TODO : 建立強型別儲存
        }
        /// <summary>
        /// 取得或設定流水號
        /// </summary>
        [Key]
        public long FileEntityStroageOperationNo { get; set; }
        /// <summary>
        /// 取得或設定檔案位於儲存個體上的橋接資訊 ID
        /// </summary>
        [Column("FileEntityStroageID")]
        public Guid FileEntityStroageId { get; set; }
        /// <summary>
        /// 取得或設定類型
        /// </summary>
        public FileEntityStorageOperationTypeEnum Type { get; set; }
        /// <summary>
        /// 取得或設定同步對象儲存個體 ID
        /// </summary>
        [Column("SyncTargetStorageID")]
        public Guid? SyncTargetStorageId { get; set; }
        /// <summary>
        /// 取得建立時間
        /// </summary>
        public DateTimeOffset CreateDateTime { get; internal set; }
        /// <summary>
        /// 取得最後更新時間
        /// </summary>
        public DateTimeOffset? UpdateDateTime { get; internal set; }
        /// <summary>
        /// 取得或設定訊息
        /// </summary>
        [Required]
        [StringLength(511)]
        public string Message { get; set; }
        /// <summary>
        /// 取得或設定擴充訊息
        /// </summary>
        [Required]
        [StringLength(511)]
        public string ExtendProperty { get; set; }
        /// <summary>
        /// 取得或設定狀態
        /// </summary>
        public FileEntityStorageOperationStatusEnum Status { get; set; }

        /// <summary>
        /// 取得或設定對應的檔案位於儲存個體上的橋接資訊
        /// </summary>
        [ForeignKey(nameof(FileEntityStroageId))]
        [InverseProperty("FileEntityStroageOperation")]
        public virtual FileEntityStorage FileEntityStroage { get; set; }
        /// <summary>
        /// 取得或設定對應的同步對象儲存個體
        /// </summary>
        [ForeignKey(nameof(SyncTargetStorageId))]
        [InverseProperty("FileEntityStroageOperation")]
        public virtual Storage Storage { get; set; }

        DateTimeOffset? ITimeStampModel.UpdateDateTime { get => this.UpdateDateTime; set => this.UpdateDateTime = value; }
        DateTimeOffset ICreatedDateModel.CreateDateTime { get => this.CreateDateTime; set => this.CreateDateTime = value; }
    }
}
