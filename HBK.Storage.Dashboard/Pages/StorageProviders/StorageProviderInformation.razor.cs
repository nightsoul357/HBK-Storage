using HBK.Storage.Dashboard.DataSource;

namespace HBK.Storage.Dashboard.Pages.StorageProviders
{
    public partial class StorageProviderInformation : HBKPageBase
    {
        /// <summary>
        /// 取得或設定正在編輯的儲存服務
        /// </summary>
        public StorageProviderResponse? StorageProviderResponse { get; set; }

        public async Task SaveChangeAsync()
        {
            if (this.StorageProviderResponse == null)
            {
                return;
            }
            var result = await base.HBKStorageApi.StorageprovidersPUTAsync(this.StorageProviderResponse.Storage_provider_id, new StorageProviderUpdateRequest()
            {
                Name = this.StorageProviderResponse.Name,
                Status = this.StorageProviderResponse.Status
            });

            base.StateContainer.StorageProviderResponse = result;
            base.Snackbar.Add("儲存成功", MudBlazor.Severity.Info);
        }

        public async Task EnableAsync()
        {
            if (this.StorageProviderResponse == null)
            {
                return;
            }

            this.StorageProviderResponse.Status.Remove(StorageProviderStatus.Disable);

            var result = await base.HBKStorageApi.StorageprovidersPUTAsync(this.StorageProviderResponse.Storage_provider_id, new DataSource.StorageProviderUpdateRequest()
            {
                Name = this.StorageProviderResponse.Name,
                Status = this.StorageProviderResponse.Status
            });

            base.StateContainer.StorageProviderResponse = result;
            base.Snackbar.Add("啟用成功", MudBlazor.Severity.Info);
        }

        public async Task DisableAsync()
        {
            if (this.StorageProviderResponse == null)
            {
                return;
            }

            this.StorageProviderResponse.Status.Add(StorageProviderStatus.Disable);

            var result = await base.HBKStorageApi.StorageprovidersPUTAsync(this.StorageProviderResponse.Storage_provider_id, new DataSource.StorageProviderUpdateRequest()
            {
                Name = this.StorageProviderResponse.Name,
                Status = this.StorageProviderResponse.Status
            });

            base.StateContainer.StorageProviderResponse = result;
            base.Snackbar.Add("停用成功", MudBlazor.Severity.Info);
        }

        public async Task ShowDeleteDialogAsync()
        {
            if (this.StorageProviderResponse == null)
            {
                return;
            }

            var result = await base.DialogService.ShowBasicAsync($"確定刪除 {this.StorageProviderResponse.Name} 嗎?", "此操作無法復原", "刪除", MudBlazor.Color.Error);
            if (!result.Cancelled)
            {
                await base.HBKStorageApi.StorageprovidersDELETEAsync(this.StorageProviderResponse.Storage_provider_id);
                base.StateContainer.StorageProviderResponse = null;
                base.NavigationManager.NavigateTo("/index");
            }
        }

        public string ConvertStatusToString(ICollection<StorageProviderStatus>? status)
        {
            if (status == null)
            {
                return String.Empty;
            }
            return status.ToList().Aggregate("", (s1, s2) => s1 + s2.ToString() + ", ").Trim().Trim(',');
        }

        protected override void OnParametersSet()
        {
            if (base.StateContainer.StorageProviderResponse == null)
            {
                base.NavigationManager.NavigateTo("/index");
                return;
            }

            this.StorageProviderResponse = base.StateContainer.StorageProviderResponse;
            base.OnParametersSet();
        }

        protected override void OnInitialized()
        {
            base.StateContainer.PropertyChanged += this.StateContainer_PropertyChanged;
            base.OnInitialized();
        }

        private void StateContainer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(base.StateContainer.StorageProviderResponse))
            {
                this.StorageProviderResponse = base.StateContainer.StorageProviderResponse;
                base.StateHasChanged();
            }
        }
    }
}
