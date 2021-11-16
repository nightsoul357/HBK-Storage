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
    /// 儲存群組擴充資訊
    /// </summary>
    public class StorageGroupExtendProperty
    {
        /// <summary>
        /// 取得或設定儲存群組
        /// </summary>
        [Sortable]
        [Filterable]
        public StorageGroup StorageGroup { get; set; }
        /// <summary>
        /// 取得或設定大小限制
        /// </summary>
        [Sortable]
        [Filterable]
        public long? SizeLimit { get; set; }
        /// <summary>
        /// 取得或設定已使用空間
        /// </summary>
        [Sortable]
        [Filterable]
        public long? UsedSize { get; set; }
    }
}
