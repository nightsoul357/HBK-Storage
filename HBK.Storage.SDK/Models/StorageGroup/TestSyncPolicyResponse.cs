using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.StorageGroup
{
    /// <summary>
    /// 測試同步策略回應內容
    /// </summary>
    public class TestSyncPolicyResponse
    {
        /// <summary>
        /// 是否通過
        /// </summary>
        [Newtonsoft.Json.JsonProperty("is_pass", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool IsPass { get; set; }
    }
}
