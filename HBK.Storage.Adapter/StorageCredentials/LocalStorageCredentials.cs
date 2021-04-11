using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.StorageCredentials
{
    /// <summary>
    /// 本地類型的存取儲存個體的驗證資訊
    /// </summary>
    public class LocalStorageCredentials : StorageCredentialsBase
    {
        /// <summary>
        /// 取得或設定目錄
        /// </summary>
        public string Directory { get; set; }
    }
}
