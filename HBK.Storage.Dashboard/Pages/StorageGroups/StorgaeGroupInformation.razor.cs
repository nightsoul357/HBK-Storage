using HBK.Storage.Dashboard.DataSource;

namespace HBK.Storage.Dashboard.Pages.StorageGroups
{
    public partial class StorgaeGroupInformation : HBKPageBase
    {
        public StorageProviderResponse? StorageProviderResponse { get; set; }
        public List<StorageGroupExtendPropertyResponse>? StorageGroupExtendPropertyResponses { get; set; }

        public async Task ShowAddStorageGroupDialogAsync()
        {
            var result = await base.DialogService.ShowAddStorageGroupDialogAsync();
            await this.UpdateStorageGroupListAsync();
        }

        public async Task ShowEditStorageGroupDialogAsync(StorageGroupExtendPropertyResponse storageGroup)
        {
            var result = await base.DialogService.ShowEditStorageGroupDialogAsync(this.ConvertToStorageGroup(storageGroup));
            await this.UpdateStorageGroupListAsync();
        }

        public async Task ShowDeleteStorageGroupDialogAsync(StorageGroupExtendPropertyResponse storageGroup)
        {
            var result = await base.DialogService.ShowBasicAsync($"確定刪除 {storageGroup.Name} 嗎?", "此操作無法復原", "刪除", MudBlazor.Color.Error);
            if (!result.Cancelled)
            {
                await base.HBKStorageApi.StoragegroupsDELETEAsync(storageGroup.Storage_group_id);
                this.Snackbar.Add("刪除成功", MudBlazor.Severity.Info);
                await this.UpdateStorageGroupListAsync();
            }
        }

        private async Task UpdateStorageGroupListAsync()
        {
            if (this.StorageProviderResponse != null)
            {
                var result = await base.HBKStorageApi.StoragegroupextendpropertiesAsync(this.StorageProviderResponse.Storage_provider_id, null, "storage_group/create_date_time", null, null);
                this.StorageGroupExtendPropertyResponses = result.Value.ToList();
                base.StateHasChanged();
            }
        }

        private void NavigetToStorageMenu(StorageGroupExtendPropertyResponse storageGroup)
        {
            base.NavigationManager.NavigateTo($"/storageGroup/{storageGroup.Storage_group_id}/storage");
        }

        private double ConvertToSizeUsedProgress(StorageGroupExtendPropertyResponse storageGroup)
        {
            if (!storageGroup.Used_size.HasValue || !storageGroup.Size_limit.HasValue)
            {
                return 0;
            }

            return Convert.ToDouble(storageGroup.Used_size.Value) / storageGroup.Size_limit.Value * 100;
        }

        protected async override Task OnParametersSetAsync()
        {
            if (base.StateContainer.StorageProviderResponse == null)
            {
                base.NavigationManager.NavigateTo("/index");
                return;
            }
            this.StorageProviderResponse = base.StateContainer.StorageProviderResponse;
            await this.UpdateStorageGroupListAsync();

            base.OnParametersSet();
        }

        protected override void OnInitialized()
        {
            base.StateContainer.PropertyChanged += this.StateContainer_PropertyChanged;
            base.OnInitialized();
        }

        private async void StateContainer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(base.StateContainer.StorageProviderResponse))
            {
                this.StorageProviderResponse = base.StateContainer.StorageProviderResponse;
                await this.UpdateStorageGroupListAsync();
            }
        }

        private StorageGroupResponse ConvertToStorageGroup(StorageGroupExtendPropertyResponse storageGroupExtendPropertyResponse)
        {
            return new StorageGroupResponse()
            {
                Clear_mode = storageGroupExtendPropertyResponse.Clear_mode,
                Clear_policy = storageGroupExtendPropertyResponse.Clear_policy,
                Create_date_time = storageGroupExtendPropertyResponse.Create_date_time,
                Download_priority = storageGroupExtendPropertyResponse.Download_priority,
                Name = storageGroupExtendPropertyResponse.Name,
                Status = storageGroupExtendPropertyResponse.Status,
                Storage_group_id = storageGroupExtendPropertyResponse.Storage_group_id,
                Storage_provider_id = storageGroupExtendPropertyResponse.Storage_provider_id,
                Sync_mode = storageGroupExtendPropertyResponse.Sync_mode,
                Sync_policy = storageGroupExtendPropertyResponse.Sync_policy,
                Type = storageGroupExtendPropertyResponse.Type,
                Update_date_time = storageGroupExtendPropertyResponse.Update_date_time,
                Upload_priority = storageGroupExtendPropertyResponse.Upload_priority
            };
        }
    }
}