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
        public Adapter.Storages.Storage Storage { get; set; }
        /// <summary>
        /// 取得或設定剩餘大小
        /// </summary>
        public long RemainSize { get; set; }
    }
}
