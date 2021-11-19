using Blazored.LocalStorage;
using Blazored.SessionStorage;
using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Services;
using System.ComponentModel;

namespace HBK.Storage.Dashboard.Models
{
    /// <summary>
    /// 狀態容易
    /// </summary>
    public class StateContainer : INotifyPropertyChanged
    {   
        private readonly ISyncSessionStorageService _sessionStorageService;
        private readonly ISyncLocalStorageService _localStorageService;
        private readonly HBKLayoutComponentService _hbkLayoutComponentService;

        private const string _signInKeyName = "_signInAuthorizeKey";
        private const string _storageProviderName = "_storgaeProvdier";

        public event PropertyChangedEventHandler? PropertyChanged;

        public StorageProviderResponse? StorageProviderResponse
        {
            get
            {
                return this.FetchData<StorageProviderResponse>(_storageProviderName);
            }
            set
            {
                var t = this.FetchData<StorageProviderResponse>(_storageProviderName);
                if (!(t != null && value != null && t.Storage_provider_id == value.Storage_provider_id))
                {
                    this.SetData(_storageProviderName, value);
                    this.NotifyPropertyChanged(nameof(this.StorageProviderResponse));
                }
            }
        }

        /// <summary>
        /// 取得或設定更新 Google 權杖回應內容
        /// </summary>
        public UpdateGoogleTokenResponse? UpdateGoogleTokenResponse
        {
            get
            {
                return this.FetchData<UpdateGoogleTokenResponse>(nameof(this.UpdateGoogleTokenResponse));
            }
            set
            {
                this.SetData(nameof(this.UpdateGoogleTokenResponse), value);
                this.NotifyPropertyChanged(nameof(this.UpdateGoogleTokenResponse));
            }
        }

        /// <summary>
        /// 初始化狀態容器
        /// </summary>
        /// <param name="sessionStorageService"></param>
        /// <param name="localStorageService"></param>
        /// <param name="hbkLayoutComponentService"></param>
        public StateContainer(ISyncSessionStorageService sessionStorageService, ISyncLocalStorageService localStorageService, HBKLayoutComponentService hbkLayoutComponentService)
        {
            _sessionStorageService = sessionStorageService;
            _localStorageService = localStorageService;
            _hbkLayoutComponentService = hbkLayoutComponentService;
        }
        /// <summary>
        /// 取得已登入的授權金鑰
        /// </summary>
        /// <returns></returns>
        public AuthorizeKeyResponse? GetSignInKey()
        {
            return this.FetchData<AuthorizeKeyResponse?>(_signInKeyName);
        }
        /// <summary>
        /// 設定已登入的授權金鑰
        /// </summary>
        /// <param name="authorizeKey"></param>
        /// <param name="isStoreInLocalStorage"></param>
        /// <returns></returns>
        public void SetSignKey(AuthorizeKeyResponse? authorizeKey, bool isStoreInLocalStorage)
        {
            this.SetData(_signInKeyName, authorizeKey, isStoreInLocalStorage);
        }

        private T FetchData<T>(string key)
        {
            var obj = _sessionStorageService.GetItem<T>(key);
            if (obj != null)
            {
                return obj;
            }
            obj = _localStorageService.GetItem<T>(key);
            if (obj != null)
            {
                _sessionStorageService.SetItem(key, obj);
            }
            return obj;
        }

        private void SetData<T>(string key, T obj, bool isStoreInLocalStorage = true)
        {
            _sessionStorageService.SetItem(key, obj);
            if (isStoreInLocalStorage)
            {
                _localStorageService.SetItem(key, obj);
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
