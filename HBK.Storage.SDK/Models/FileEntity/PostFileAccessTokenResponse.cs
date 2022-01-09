using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.FileEntity
{
    /// <summary>
    /// 產生檔案存取權杖回應內容
    /// </summary>
    public partial class PostFileAccessTokenResponse
    {
        /// <summary>
        /// 取得或設定權杖
        /// </summary>
        [Newtonsoft.Json.JsonProperty("token", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Token { get; set; }

        /// <summary>
        /// 取得或設定過期時間
        /// </summary>
        [Newtonsoft.Json.JsonProperty("expire_date_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public DateTimeOffset ExpireDateTime { get; set; }
    }
}
