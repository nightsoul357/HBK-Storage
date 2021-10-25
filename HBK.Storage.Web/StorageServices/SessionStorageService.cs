using Blazored.SessionStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.StorageServices
{
    public class SessionStorageService : IStorageService
    {
        private readonly ISyncSessionStorageService _sessionStorageService;
        public SessionStorageService(ISyncSessionStorageService sessionStorageService)
        {
            _sessionStorageService = sessionStorageService;
        }
        public T GetItem<T>(string key)
        {
            return _sessionStorageService.GetItem<T>(key);
        }

        public void SetItem<T>(string key, T data)
        {
            _sessionStorageService.SetItem(key, data);
        }
    }
}
