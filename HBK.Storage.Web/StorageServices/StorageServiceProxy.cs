using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.StorageServices
{
    public class StorageServiceProxy : IStorageService
    {
        private readonly IStorageService _storageService;
        private readonly IStorageService _internalStorageService;
        public StorageServiceProxy(IStorageService storageService, IStorageService internalStorageService)
        {
            _storageService = storageService;
            _internalStorageService = internalStorageService;
        }
        public T GetItem<T>(string key)
        {
            T result = _storageService.GetItem<T>(key);
            if (result != null || _internalStorageService == null)
            {
                return result;
            }
            return _internalStorageService.GetItem<T>(key);
        }
        public void SetItem<T>(string key, T data)
        {
            _storageService.SetItem(key, data);
            if (_internalStorageService != null)
            {
                _internalStorageService.SetItem(key, data);
            }
        }

        public void SetItemWithoutRecursive<T>(string key, T data)
        {
            _storageService.SetItem(key, data);
        }
    }
}
