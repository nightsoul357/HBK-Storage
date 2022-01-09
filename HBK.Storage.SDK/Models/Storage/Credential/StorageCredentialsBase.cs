using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.Storage.Credential
{
    /// <summary>
    /// 存取儲存個體的驗證資訊基底型別
    /// </summary>
    public class StorageCredentialsBase
    {
        [Newtonsoft.Json.JsonProperty("storage_type", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public StorageType StorageType { get; set; }
    }
}
