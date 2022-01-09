using HBK.Storage.SDK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK
{
    /// <summary>
    /// HBK Storage 服務
    /// </summary>
    public class HBKStorageClient : ServiceBase
    {
        public HBKStorageClient(string baseUrl, Func<HttpClient> httpClientFunc)
            : base(baseUrl, httpClientFunc)
        {
        }

        public HBKStorageClient(string baseUrl, Func<HttpClient> httpClientFunc, string apiKey)
            : base(baseUrl, httpClientFunc, apiKey)
        {
        }
        /// <summary>
        /// 建置直接下載連結
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="filename">檔案名稱</param>
        /// <param name="token">存取權杖</param>
        /// <returns>直接下載連結</returns>
        public string BuildDirectDownloadLink(Guid? fileEntityId, string? filename, string? token)
        {
            var urlBuilder_ = new StringBuilder();
            if (fileEntityId != null && !string.IsNullOrWhiteSpace(filename))
            {
                urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/docs/{fileEntityId}/filename/{filename}");
                urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));
                urlBuilder_.Replace("{filename}", filename);
            }
            else if (fileEntityId == null && !string.IsNullOrWhiteSpace(filename))
            {
                urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/docs/filename/{filename}");
                urlBuilder_.Replace("{filename}", filename);
            }
            else if (fileEntityId != null && string.IsNullOrWhiteSpace(filename))
            {
                urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/docs/{fileEntityId}");
                urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));
            }
            else if (fileEntityId == null && string.IsNullOrWhiteSpace(filename))
            {
                urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/docs");
            }

            if (!string.IsNullOrEmpty(token))
            {
                urlBuilder_.Append("?").Append(System.Uri.EscapeDataString("esic") + "=").Append(System.Uri.EscapeDataString(token));
            }

            return urlBuilder_.ToString();
        }
        /// <summary>
        /// 取得儲存服務
        /// </summary>
        public StorageProviderService StorageProvider
        {
            get
            {
                return new StorageProviderService(base.BaseUrl, base._httpClientFunc, base._apiKey);
            }
        }

        /// <summary>
        /// 取得儲存群組服務
        /// </summary>
        public StorageGroupService StorageGroup
        {
            get
            {
                return new StorageGroupService(base.BaseUrl, base._httpClientFunc, base._apiKey);
            }
        }

        /// <summary>
        /// 取得儲存個體服務
        /// </summary>
        public StorageService Storage
        {
            get
            {
                return new StorageService(base.BaseUrl, base._httpClientFunc, base._apiKey);
            }
        }

        /// <summary>
        /// 取得檔案服務
        /// </summary>
        public FileEntityService FileEntity
        {
            get
            {
                return new FileEntityService(base.BaseUrl, base._httpClientFunc, base._apiKey);
            }
        }

        /// <summary>
        /// 取得檔案權杖服務
        /// </summary>
        public FileAccessTokenService FileAccessToken
        {
            get
            {
                return new FileAccessTokenService(base.BaseUrl, base._httpClientFunc, base._apiKey);
            }
        }

        /// <summary>
        /// 取得驗證金鑰服務
        /// </summary>
        public AuthorizeKeyService AuthorizeKey
        {
            get
            {
                return new AuthorizeKeyService(base.BaseUrl, base._httpClientFunc, base._apiKey);
            }
        }
    }
}
