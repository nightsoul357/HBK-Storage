using HBK.Storage.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models.FileEntity
{
    /// <summary>
    /// 更新檔案實體請求內容
    /// </summary>
    public class FileEntityUpdateRequest
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("filename", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Filename { get; set; }

        /// <summary>
        /// 擴充資訊
        /// </summary>
        [Newtonsoft.Json.JsonProperty("extend_property", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string ExtendProperty { get; set; }

        /// <summary>
        /// 存取模式
        /// </summary>
        [Newtonsoft.Json.JsonProperty("access_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public AccessType AccessType { get; set; }
    }
}
