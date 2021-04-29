using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.StorageCredentials
{
    /// <summary>
    /// FTP 類型的存取儲存個體的驗證資訊
    /// </summary>
    public class FTPStorageCredentials : StorageCredentialsBase
    {
        /// <summary>
        /// 取得或設定 Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 取得或設定密碼
        /// </summary>
        public string Password { get; set; }
    }
}
