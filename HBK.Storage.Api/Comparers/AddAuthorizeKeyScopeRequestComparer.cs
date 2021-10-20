using HBK.Storage.Api.Models.AuthorizeKey;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Comparers
{
    /// <summary>
    /// 新增驗證金鑰使用範圍請求內容比較方法
    /// </summary>
    public class AddAuthorizeKeyScopeRequestComparer : IEqualityComparer<AddAuthorizeKeyScopeRequest>
    {
        private static Lazy<AddAuthorizeKeyScopeRequestComparer> Comparer =
            new Lazy<AddAuthorizeKeyScopeRequestComparer>(() => new AddAuthorizeKeyScopeRequestComparer());
        private AddAuthorizeKeyScopeRequestComparer()
        {

        }
        /// <inheritdoc/>
        public bool Equals(AddAuthorizeKeyScopeRequest x, AddAuthorizeKeyScopeRequest y)
        {
            return x.AllowOperationType == y.AllowOperationType
                && x.StorageProviderId == y.StorageProviderId;
        }

        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] AddAuthorizeKeyScopeRequest obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        /// 取得唯一實體
        /// </summary>
        /// <returns></returns>
        public static AddAuthorizeKeyScopeRequestComparer GetInstance()
        {
            return AddAuthorizeKeyScopeRequestComparer.Comparer.Value;
        }
    }
}
