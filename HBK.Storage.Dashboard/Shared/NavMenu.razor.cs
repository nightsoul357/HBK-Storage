using HBK.Storage.Dashboard.DataSource;

namespace HBK.Storage.Dashboard.Shared
{
    public partial class NavMenu : HBKLayoutComponentBase
    {
        /// <summary>
        /// 取得或設定儲存服務清單
        /// </summary>
        public List<StorageProviderResponse> StorageProviderResponses { get; set; } = new List<StorageProviderResponse>();

        protected async override Task OnParametersSetAsync()
        {
            this.StorageProviderResponses = (await base.HBKStorageApi.StorageprovidersGET2Async(null, null, null, null)).Value.ToList();
            if (base.StateContainer.StorageProviderResponse != null)
            {
                base.StateContainer.StorageProviderResponse = this.StorageProviderResponses.FirstOrDefault(x => x.Storage_provider_id == base.StateContainer.StorageProviderResponse.Storage_provider_id);
            }
            await base.OnParametersSetAsync();
        }

        public void SelectStorageProvider(StorageProviderResponse storageProviderResponse)
        {
            base.StateContainer.StorageProviderResponse = storageProviderResponse;
        }

        public async Task ShowAddStorageProviderDialogAsync()
        {
            var result = await base.DialogService.ShowAddStorageProviderDialogAsync();
            if (!result.Cancelled)
            {
                this.StorageProviderResponses = (await base.HBKStorageApi.StorageprovidersGET2Async(null, null, null, null)).Value.ToList();
                base.StateContainer.StorageProviderResponse = result.Data;
            }
        }

        public string ConvertToTitle(StorageProviderResponse? storageProvider)
        {
            if (storageProvider == null)
            {
                return "Please Select";
            }
            else if (storageProvider.Name.Length > 6)
            {
                return storageProvider.Name;
            }
            else
            {
                return storageProvider.Name + new string('　', 6 - storageProvider.Name.Length);
            }
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
                this.UpdateStorageProviderAsync();
                base.StateHasChanged();
            }
        }

        private async Task UpdateStorageProviderAsync()
        {
            this.StorageProviderResponses = (await base.HBKStorageApi.StorageprovidersGET2Async(null, null, null, null)).Value.ToList();
        }
    }
}
