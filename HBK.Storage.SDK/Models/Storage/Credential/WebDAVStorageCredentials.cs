namespace HBK.Storage.SDK.Models.Storage.Credential
{
    /// <summary>
    /// Web DAV 使用的驗證資訊
    /// </summary>
    public class WebDAVStorageCredentials : StorageCredentialsBase
    {
        public WebDAVStorageCredentials()
        {
            base.StorageType = Enums.StorageType.WebDav;
        }
        /// <summary>
        /// 取得或設定 URL
        /// </summary>
        [Newtonsoft.Json.JsonProperty("url", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Url { get; set; }
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        [Newtonsoft.Json.JsonProperty("username", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Username { get; set; }
        /// <summary>
        /// 取得或設定密碼
        /// </summary>
        [Newtonsoft.Json.JsonProperty("password", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Password { get; set; }
        /// <summary>
        /// 取得或設定是否支援分段上傳
        /// </summary>
        [Newtonsoft.Json.JsonProperty("is_support_partial_upload", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool IsSupportPartialUpload { get; set; } = false;
    }
}
