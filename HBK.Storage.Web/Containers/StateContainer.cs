using Blazored.LocalStorage;
using Blazored.SessionStorage;
using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.StorageServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Containers
{
    /// <summary>
    /// 狀態容器
    /// </summary>
    public sealed class StateContainer
    {
        private readonly StorageServiceProxy _storageServiceProxy;
        public StateContainer(StorageServiceProxy storageServiceProxy)
        {
            _storageServiceProxy = storageServiceProxy;
        }
        /// <summary>
        /// 取得或設定已登入的驗證金鑰
        /// </summary>
        public AuthorizeKeyResponse AuthorizeKey
        {
            get
            {
                return _storageServiceProxy.GetItem<AuthorizeKeyResponse>(nameof(this.AuthorizeKey));
            }
            set
            {
                _storageServiceProxy.SetItem(nameof(this.AuthorizeKey), value);
            }
        }
        /// <summary>
        /// 取得或設定選擇的儲存提供者
        /// </summary>
        public StorageProviderResponse StorageProvider
        {
            get
            {
                return _storageServiceProxy.GetItem<StorageProviderResponse>(nameof(this.StorageProvider));
            }
            set
            {
                _storageServiceProxy.SetItem(nameof(this.StorageProvider), value);
            }
        }

        public StorageGroupExtendPropertyResponse StorageGroup
        {
            get
            {
                return _storageServiceProxy.GetItem<StorageGroupExtendPropertyResponse>(nameof(this.StorageGroup));
            }
            set
            {
                _storageServiceProxy.SetItem(nameof(this.StorageGroup), value);
            }
        }

        public void SetAuthoirzeKey(AuthorizeKeyResponse authorizeKey, bool isRemeber)
        {
            if (isRemeber)
            {
                this.AuthorizeKey = authorizeKey;
            }
            else
            {
                _storageServiceProxy.SetItemWithoutRecursive(nameof(this.AuthorizeKey), authorizeKey);
            }
        }
    }
}
