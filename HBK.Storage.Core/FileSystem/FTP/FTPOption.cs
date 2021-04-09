using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.FTP
{
    /// <summary>
    /// FTP 連接選項
    /// </summary>
    public class FTPOption
    {
        /// <summary>
        /// 取得或設定 Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 取得或設定連接 FTP 使用的驗證
        /// </summary>
        public ICredentials Credentials { get; set; }
        /// <summary>
        /// 取得或設定上傳時使用的緩衝區大小
        /// </summary>
        public int UploadBufferSize { get; set; } = 1024 * 1024;
    }
}
