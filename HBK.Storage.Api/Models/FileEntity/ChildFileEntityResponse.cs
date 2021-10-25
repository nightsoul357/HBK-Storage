using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileEntity
{
    /// <summary>
    /// Child 的檔案實體回應內容
    /// </summary>
    public class ChildFileEntityResponse : FileEntityResponse
    {
        /// <summary>
        /// 根檔案 ID
        /// </summary>
        public Guid? RootFileEntityId { get; set; }
        /// <summary>
        /// 與根檔案相差的層數
        /// </summary>
        public int? ChildLevel { get; set; }
    }
}
