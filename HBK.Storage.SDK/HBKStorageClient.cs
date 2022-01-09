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
