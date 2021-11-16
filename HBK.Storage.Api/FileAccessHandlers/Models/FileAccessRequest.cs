using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers.Models
{
    /// <summary>
    /// 檔案處理請求內容
    /// </summary>
    public class FileAccessRequest
    {
        /// <summary>
        /// 取得或設定驗證資訊
        /// </summary>
        public string Esic { get; set; }
        /// <summary>
        /// 取得或設定檔案實體 ID
        /// </summary>
        public Guid? FileEntityId { get; set; }
        /// <summary>
        /// 取得或設定 HTTP 請求內容
        /// </summary>
        public HttpRequest HttpRequest { get; set; }
    }
}
