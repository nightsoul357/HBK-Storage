using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileEntity
{
    /// <summary>
    /// 附加標籤請求內容
    /// </summary>
    public class AppendTagRequest
    {
        /// <summary>
        /// 取得或設定標籤
        /// </summary>
        public string Tag { get; set; }
    }
}
