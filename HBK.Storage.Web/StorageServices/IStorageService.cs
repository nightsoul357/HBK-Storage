using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.StorageServices
{
    public interface IStorageService
    {
        T GetItem<T>(string key);
        void SetItem<T>(string key, T data);
    }
}
