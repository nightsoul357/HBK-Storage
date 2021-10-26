using HBK.Storage.Adapter.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Models
{
    /// <summary>
    /// 儲存個體延展資訊
    /// </summary>
    public class StorageExtendProperty
    {
        /// <summary>
        /// 取得過設定儲存個體
        /// </summary>
        [Sortable]
        [Filterable]
        public Adapter.Storages.Storage Storage { get; set; }
        /// <summary>
        /// 取得或設定已使用大小(Bytes)
        /// </summary>
        [Sortable]
        [Filterable]
        public long? UsedSize { get; set; }
        /// <summary>
        /// 取得剩餘大小(Bytes)
        /// </summary>
        public long RemainSize
        {
            get
            {
                return this.Storage.SizeLimit - (this.UsedSize ?? 0);
            }
        }
    }
}
