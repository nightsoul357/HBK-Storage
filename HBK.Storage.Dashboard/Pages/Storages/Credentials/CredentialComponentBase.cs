using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;

namespace HBK.Storage.Dashboard.Pages.Storages.Credentials
{
    public class CredentialComponentBase<T> : HBKLayoutComponentBase, ICredentialsComponent
        where T : StorageCredentialsBase
    {
        private bool _ifFirstSet = true;
        protected T _credentials;

        [Parameter]
        public StorageCredentialsBase Credential
        {
            get
            {
                return _credentials;
            }
            set
            {
                if (_ifFirstSet)
                {
                    _credentials = (T)value;
                    _ifFirstSet = false;
                }
            }
        }
    }
}
