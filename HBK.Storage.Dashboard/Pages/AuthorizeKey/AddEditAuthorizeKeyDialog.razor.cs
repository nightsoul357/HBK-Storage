using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HBK.Storage.Dashboard.Pages.AuthorizeKey
{
    public partial class AddEditAuthorizeKeyDialog : HBKDialogBase
    {
        [Parameter]
        public AuthorizeKeyResponse EditAuthorizeKey { get; set; }
        public List<StorageProviderResponse> StorageProviders { get; set; }

        public string KeyValue { get; set; }
        public bool IsEditMode
        {
            get
            {
                return this.EditAuthorizeKey != null;
            }
        }
        public string Name { get; set; }
        public AuthorizeKeyType AuthorizeKeyType { get; set; }
        public bool IsRead { get; set; }
        public bool IsInsert { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public void Cancel()
        {
            this.MudDialog.Cancel();
        }
        public async Task SaveChangeAsync()
        {
            if (!this.IsEditMode)
            {
                List<AddAuthorizeKeyScopeRequest> addAuthorizeKeyScopeRequests = new List<AddAuthorizeKeyScopeRequest>();
                if (this.IsRead)
                {
                    addAuthorizeKeyScopeRequests.Add(new AddAuthorizeKeyScopeRequest()
                    {
                        Storage_provider_id = this.StateContainer.StorageProviderResponse.Storage_provider_id,
                        Allow_operation_type = AuthorizeKeyScopeOperationType.Read
                    });
                }
                if (this.IsInsert)
                {
                    addAuthorizeKeyScopeRequests.Add(new AddAuthorizeKeyScopeRequest()
                    {
                        Storage_provider_id = this.StateContainer.StorageProviderResponse.Storage_provider_id,
                        Allow_operation_type = AuthorizeKeyScopeOperationType.Insert
                    });
                }
                if (this.IsUpdate)
                {
                    addAuthorizeKeyScopeRequests.Add(new AddAuthorizeKeyScopeRequest()
                    {
                        Storage_provider_id = this.StateContainer.StorageProviderResponse.Storage_provider_id,
                        Allow_operation_type = AuthorizeKeyScopeOperationType.Update
                    });
                }
                if (this.IsDelete)
                {
                    addAuthorizeKeyScopeRequests.Add(new AddAuthorizeKeyScopeRequest()
                    {
                        Storage_provider_id = this.StateContainer.StorageProviderResponse.Storage_provider_id,
                        Allow_operation_type = AuthorizeKeyScopeOperationType.Delete
                    });
                }
                var result = await this.HBKStorageApi.AuthorizekeyPOSTAsync(new AddAuthorizeKeyRequest()
                {
                    Name = this.Name,
                    Type = this.AuthorizeKeyType,
                    Add_authorize_key_scope_requests = addAuthorizeKeyScopeRequests
                });
                this.KeyValue = result.Key_value;
                base.StateHasChanged();
            }
        }
        public async Task EnableAsync()
        {
            await this.HBKStorageApi.EnableAsync(this.EditAuthorizeKey.Authorize_key_id);
            this.MudDialog.Close(DialogResult.Ok(true));
        }
        public async Task DisableAsync()
        {
            if (this.EditAuthorizeKey.Authorize_key_id == this.StateContainer.GetSignInKey().Authorize_key_id)
            {
                this.Snackbar.Add("無法停用目前的驗證金鑰", Severity.Error);
                return;
            }
            await this.HBKStorageApi.DisableAsync(this.EditAuthorizeKey.Authorize_key_id);
            this.MudDialog.Close(DialogResult.Ok(true));
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (this.EditAuthorizeKey != null)
            {
                this.Name = this.EditAuthorizeKey.Name;
                this.AuthorizeKeyType = this.EditAuthorizeKey.Type;
            }
        }
    }
}
