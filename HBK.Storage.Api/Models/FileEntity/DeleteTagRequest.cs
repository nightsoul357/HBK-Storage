using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileEntity
{
    /// <summary>
    /// 刪除檔案實體標籤請求內容
    /// </summary>
    public class DeleteTagRequest
    {
        /// <summary>
        /// 取得或設定標籤
        /// </summary>
        public string Tag { get; set; }
    }
}
