using HBK.Storage.Adapter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.StorageCredentials
{
    /// <summary>
    /// 存取儲存個體的驗證資訊基底型別
    /// </summary>
    public abstract class StorageCredentialsBase
    {
        /// <summary>
        /// 取得或設定儲存個體類型
        /// </summary>
        public StorageTypeEnum StorageType { get; set; }
    }
}
