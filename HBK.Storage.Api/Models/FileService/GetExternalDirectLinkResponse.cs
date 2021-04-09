using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileService
{
    /// <summary>
    /// 取得外部檔案直連位置回應內容
    /// </summary>
    public class GetExternalDirectLinkResponse
    {
        /// <summary>
        /// 取得或設定檔案直連位置
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 取得或設定過期時間
        /// </summary>
        public DateTime ExpireDateTime { get; set; }
    }
}
