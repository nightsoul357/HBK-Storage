using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.StorageGroup
{
    /// <summary>
    /// 同步規則請求內容
    /// </summary>
    public class SyncPolicyRequest
    {
        /// <summary>
        /// 規則
        /// </summary>
        [Newtonsoft.Json.JsonProperty("rule", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Rule { get; set; }
    }
}
