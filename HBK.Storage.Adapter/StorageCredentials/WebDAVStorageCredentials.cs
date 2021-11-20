using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.StorageCredentials
{
    /// <summary>
    /// Web DAV 使用的驗證資訊
    /// </summary>
    public class WebDAVStorageCredentials : StorageCredentialsBase
    {
        /// <summary>
        /// 取得或設定 URL
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
        /// <summary>
        /// 取得或設定是否支援分段上傳
        /// </summary>
        public bool IsSupportPartialUpload { get; set; } = false;
    }
}
