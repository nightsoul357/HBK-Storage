using HBK.Storage.Adapter.DataAnnotations;
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
    /// 檔案實體標籤
    /// </summary>
    public class FileEntityTag
    {
        /// <summary>
        /// 取得或設定流水號
        /// </summary>
        [Key]
        public long FileEntityTagNo { get; set; }
        /// <summary>
        /// 取得或設定檔案實體 ID
        /// </summary>
        [Column("FileEntityID")]
        public Guid FileEntityId { get; set; }
        /// <summary>
        /// 取得或設定值
        /// </summary>
        [Required]
        [Filterable]
        public string Value { get; set; }

        /// <summary>
        /// 取得或設定檔案實體
        /// </summary>
        [ForeignKey(nameof(FileEntityId))]
        [InverseProperty("FileEntityTag")]
        public virtual FileEntity FileEntity { get; set; }
    }
}
