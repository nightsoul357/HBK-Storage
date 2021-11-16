using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.StorageServices
{
    public class LocalStorageService : IStorageService
    {
        private readonly ISyncLocalStorageService _localStorageService;
        public LocalStorageService(ISyncLocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }
        public T GetItem<T>(string key)
        {
            return _localStorageService.GetItem<T>(key);
        }

        public void SetItem<T>(string key, T data)
        {
            _localStorageService.SetItem(key, data);
        }
    }
}
