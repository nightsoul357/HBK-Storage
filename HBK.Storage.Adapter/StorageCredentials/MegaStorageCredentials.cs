using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.StorageCredentials
{
    /// <summary>
    /// Mega 使用的驗證資訊
    /// </summary>
    public class MegaStorageCredentials : StorageCredentialsBase
    {
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 取得或設定密碼
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 取得或設定父系資料夾 ID
        /// </summary>
        public string ParentId { get; set; }
    }
}
