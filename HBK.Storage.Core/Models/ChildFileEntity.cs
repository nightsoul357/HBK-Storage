using HBK.Storage.Adapter.DataAnnotations;
using HBK.Storage.Adapter.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Models
{
    /// <summary>
    /// Child 的檔案實體
    /// </summary>
    public class ChildFileEntity
    {
        /// <summary>
        /// 取得或設定根檔案 ID
        /// </summary>
        public Guid? RootFileEntityId { get; set; }
        /// <summary>
        /// 取得或設定檔案實體
        /// </summary>
        [Sortable]
        [Filterable]
        public FileEntity FileEntity { get; set; }
        /// <summary>
        /// 取得或設定於根檔案相距層數
        /// </summary>
        [Sortable]
        [Filterable]
        public int? ChildLevel { get; set; }
    }
}
