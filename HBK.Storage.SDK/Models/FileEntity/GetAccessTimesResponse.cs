using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.FileEntity
{
    /// <summary>
    /// 取得檔案存取次數回應內容
    /// </summary>
    public class GetAccessTimesResponse
    {
        /// <summary>
        /// 總存取次數
        /// </summary>
        [Newtonsoft.Json.JsonProperty("total_access_times", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long TotalAccessTimes { get; set; }
    }
}
