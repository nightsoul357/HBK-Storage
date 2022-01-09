using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models
{
    /// <summary>
    /// 分頁後的回應結果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// 取得或設定資料
        /// </summary>
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<T> Value { get; set; }

        /// <summary>
        /// 取得或設定資料總數
        /// </summary>
        [Newtonsoft.Json.JsonProperty("@odata.count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int OdataCount { get; set; }
    }
}
