using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HBK.Storage.Api.Models
{
    /// <summary>
    /// 分頁後的回應結果
    /// </summary>
    [JsonObject]
    public class PagedResponse<T> : IEnumerable<T>
    {
        /// <summary>
        /// 取得或設定資料
        /// </summary>
        public IEnumerable<T> Value { get; set; }

        /// <summary>
        /// 取得或設定資料總數
        /// </summary>
        /// <example>100</example>
        [JsonProperty("@odata.count")]
        public int TotalCount { get; set; }

        /// <summary>
        /// 傳回逐一查看集合的列舉值。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.Value.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}