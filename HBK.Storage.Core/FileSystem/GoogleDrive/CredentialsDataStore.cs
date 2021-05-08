using Google.Apis.Util.Store;
using HBK.Storage.Adapter.StorageCredentials;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.GoogleDrive
{
    /// <summary>
    /// Google Drive Token 儲存方式
    /// </summary>
    public class CredentialsDataStore : IDataStore
    {
        private Adapter.Storages.Storage _storage;
        private StorageService _storageService;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="storage">儲存個體</param>
        /// <param name="storageService">儲存個體服務</param>
        public CredentialsDataStore(Adapter.Storages.Storage storage, StorageService storageService)
        {
            _storage = storage;
            _storageService = storageService;
        }

        /// <inheritdoc/>
        public async Task ClearAsync()
        {
            GoogleDriveCredentials credentials = (GoogleDriveCredentials)_storage.Credentials;
            credentials.Tokens.Clear();
            _storage = await _storageService.UpdateCredentialsAsync(_storage.StorageId, credentials);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync<T>(string key)
        {
            GoogleDriveCredentials credentials = (GoogleDriveCredentials)_storage.Credentials;
            credentials.Tokens.Remove(key);
            _storage = await _storageService.UpdateCredentialsAsync(_storage.StorageId, credentials);
        }

        /// <inheritdoc/>
        public Task<T> GetAsync<T>(string key)
        {
            return Task<T>.Run(() =>
            {
                GoogleDriveCredentials credentials = (GoogleDriveCredentials)_storage.Credentials;
                if (!credentials.Tokens.ContainsKey(key) || String.IsNullOrWhiteSpace(credentials.Tokens[key]))
                {
                    return default(T);
                }
                return JsonConvert.DeserializeObject<T>(credentials.Tokens[key]);
            });
        }

        /// <inheritdoc/>
        public async Task StoreAsync<T>(string key, T value)
        {
            GoogleDriveCredentials credentials = (GoogleDriveCredentials)_storage.Credentials;
            credentials.Tokens[key] = JsonConvert.SerializeObject(value);
            _storage = await _storageService.UpdateCredentialsAsync(_storage.StorageId, credentials);
        }
    }
}
