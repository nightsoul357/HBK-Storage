using HBK.Storage.Adapter.StorageCredentials;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.ValueConversions
{
    /// <summary>
    /// 存取儲存個體的驗證資訊比較方法
    /// </summary>
    public class StorageCredentialsValueComparer : ValueComparer<StorageCredentialsBase>
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public StorageCredentialsValueComparer()
            : base((t1, t2) => DoEquals(t1, t2),
                    t => DoGetHashCode(t),
                    t => DoGetSnapshot(t))
        {
        }
        private static StorageCredentialsBase DoGetSnapshot(StorageCredentialsBase instance)
        {
            var result = (StorageCredentialsBase)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(instance), instance.GetType());
            return result;
        }
        private static int DoGetHashCode(StorageCredentialsBase instance)
        {
            return JsonConvert.SerializeObject(instance).GetHashCode();
        }
        private static bool DoEquals(StorageCredentialsBase left, StorageCredentialsBase right)
        {
            var result = JsonConvert.SerializeObject(left).Equals(JsonConvert.SerializeObject(right));
            return result;
        }
    }
}
