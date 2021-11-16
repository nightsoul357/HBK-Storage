using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HBK.Storage.Adapter.Storages
{
    /// <summary>
    /// 儲存個體分析結果
    /// </summary>
    public partial class VwStorageAnalysis
    {
        /// <summary>
        /// 取得或設定儲存個體 ID
        /// </summary>
        [Key]
        [Column("StorageID")]
        public Guid StorageId { get; set; }
        /// <summary>
        /// 取得或設定已使用空間(Bytes)
        /// </summary>
        public long? UsedSize { get; set; }
    }
}
