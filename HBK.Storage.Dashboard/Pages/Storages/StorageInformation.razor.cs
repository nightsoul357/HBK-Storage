using HBK.Storage.Dashboard.DataSource;
using Microsoft.AspNetCore.Components;

namespace HBK.Storage.Dashboard.Pages.Storages
{
    public partial class StorageInformation : HBKPageBase
    {
        public StorageGroupResponse? StorageGroupResponse { get; set; }
        public List<StorageExtendPropertyResponse>? StorageExtendPropertyResponses { get; set; }
        [Parameter]
        public string StorageGroupId { get; set; }

        private async Task ShowAddStorageDialogAsync()
        {
            if (this.StorageGroupResponse == null)
            {
                return;
            }

            var result = await base.DialogService.ShowAddStorageDialogAsync(this.StorageGroupResponse.Storage_group_id);
            await this.UpdateAsync();
        }

        private void GoBack()
        {
            base.NavigationManager.NavigateTo("/storageGroup");
        }

        private string ConvertStatusToString(ICollection<StorageStatus> statuses)
        {
            return statuses.Aggregate("", (s1, s2) => s1 + s2 + ", ").Trim().Trim(',');
        }
        private async Task ShowEditStorageDialogAsync(StorageExtendPropertyResponse storageExtendPropertyResponse)
        {
            if (this.StorageGroupResponse == null)
            {
                return;
            }

            var result = await base.DialogService.ShowEditStorageDialogAsync(this.StorageGroupResponse.Storage_group_id, this.ConvertToStorage(storageExtendPropertyResponse));
            await this.UpdateAsync();
        }
        private StorageResponse ConvertToStorage(StorageExtendPropertyResponse storageExtendPropertyResponse)
        {
            return new StorageResponse()
            {
                Create_date_time = storageExtendPropertyResponse.Create_date_time,
                Credentials = storageExtendPropertyResponse.Credentials,
                Name = storageExtendPropertyResponse.Name,
                Size_limit = storageExtendPropertyResponse.Size_limit,
                Status = storageExtendPropertyResponse.Status,
                Storage_group_response = storageExtendPropertyResponse.Storage_group_response,
                Storage_id = storageExtendPropertyResponse.Storage_id,
                Type = storageExtendPropertyResponse.Type,
                Update_date_time = storageExtendPropertyResponse.Update_date_time
            };
        }
        private double ConvertToSizeUsedProgress(StorageExtendPropertyResponse storage)
        {
            if (!storage.Used_size.HasValue)
            {
                return 0;
            }

            return Convert.ToDouble(storage.Used_size.Value) / storage.Size_limit * 100;
        }

        protected async override Task OnParametersSetAsync()
        {
            await this.UpdateAsync();
            await base.OnParametersSetAsync();
        }

        private async Task UpdateAsync()
        {
            if (this.StorageGroupId != null && this.StorageGroupResponse == null)
            {
                this.StorageGroupResponse = await base.HBKStorageApi.StoragegroupsGETAsync(Guid.Parse(this.StorageGroupId));
            }
            if (this.StorageGroupResponse != null)
            {
                this.StorageExtendPropertyResponses = (await base.HBKStorageApi.StorageextendpropertiesAsync(this.StorageGroupResponse.Storage_group_id, null, null, null, null)).Value.ToList();
            }
            base.StateHasChanged();
        }
    }
}
