using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileEntity
{
    /// <summary>
    /// 取得檔案存取次數回應內容
    /// </summary>
    public class GetAccessTimesResponse
    {
        /// <summary>
        /// 總存取次數
        /// </summary>
        public long TotalAccessTimes { get; set; }
    }
}
