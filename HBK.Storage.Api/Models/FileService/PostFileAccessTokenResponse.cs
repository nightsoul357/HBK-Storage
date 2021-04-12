using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileService
{
    /// <summary>
    /// 產生檔案存取權杖回應內容
    /// </summary>
    public class PostFileAccessTokenResponse
    {
        /// <summary>
        /// 取得或設定權杖
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 取得或設定過期時間
        /// </summary>
        public DateTime ExpireDateTime { get; set; }
        /// <summary>
        /// 取得或設定 URL 格式 A
        /// </summary>
        public string UrlParrtenA { get; set; }
        /// <summary>
        /// 取得或設定 URL 格式 B
        /// </summary>
        public string UrlParrtenB { get; set; }
    }
}
