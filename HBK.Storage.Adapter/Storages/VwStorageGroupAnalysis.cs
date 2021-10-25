using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HBK.Storage.Adapter.Storages
{
    /// <summary>
    /// 儲存群組的分析結果
    /// </summary>
    public partial class VwStorageGroupAnalysis
    {
        /// <summary>
        /// 取得或設定儲存群組 ID
        /// </summary>
        [Key]
        [Column("StorageGroupID")]
        public Guid StorageGroupId { get; set; }
        /// <summary>
        /// 取得或設定大小限制
        /// </summary>
        public long? SizeLimit { get; set; }
        /// <summary>
        /// 取得或設定已使用大小
        /// </summary>
        public long? UsedSize { get; set; }
    }
}
