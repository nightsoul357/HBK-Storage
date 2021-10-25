using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.StorageCredentials
{
    /// <summary>
    /// Google Drive 使用的驗證資訊
    /// </summary>
    public class GoogleDriveCredentials : StorageCredentialsBase
    {
        /// <summary>
        /// 初始化 Google Drive 使用的驗證資訊
        /// </summary>
        public GoogleDriveCredentials()
        {
            this.Tokens = new Dictionary<string, string>();
        }
        /// <summary>
        /// 取得或設定父資料夾 ID
        /// </summary>
        public string Parent { get; set; }
        /// <summary>
        /// 取得或設定存取金鑰
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 取得或設定存取密鑰
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 取得或設定 Token 資訊
        /// </summary>
        public Dictionary<string,string> Tokens { get; set; }
    }
}
