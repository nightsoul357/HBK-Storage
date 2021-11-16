using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.WebDAV
{
    /// <summary>
    /// WebDAV 連接選項
    /// </summary>
    public class WebDAVOption
    {
        /// <summary>
        /// 初始化 WebDAV 連接選項
        /// </summary>
        public WebDAVOption()
        {
            this.HttpClientFunc = new Func<HttpClient>(() =>
            {
                return new HttpClient();
            });
        }
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
        /// 取得 Http Client 產生方法
        /// </summary>
        public Func<HttpClient> HttpClientFunc { get; set; }
    }
}
