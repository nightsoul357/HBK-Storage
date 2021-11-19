using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;

namespace HBK.Storage.Dashboard.Pages.StorageGroups
{
    public partial class AddEditStorageGroupDialog : HBKDialogBase
    {
        [Parameter]
        public StorageGroupResponse StorageGroupResponse { get; set; } = new StorageGroupResponse()
        {
            Clear_policy = new ClearPolicy(),
            Sync_policy = new SyncPolicy(),
            Status = new List<StorageGroupStatus>(),
            Download_priority = 1,
            Upload_priority = 1,
            Clear_mode = ClearMode.Stop,
            Sync_mode = SyncMode.Never
        };
        [Parameter]
        public bool IsEditMode { get; set; } = false;

        public async Task DisableAsnyc()
        {
            this.StorageGroupResponse.Status.Add(StorageGroupStatus.Disable);
            await this.UpdateAsync();
            base.Snackbar.Add("停用成功", MudBlazor.Severity.Info);
            base.StateHasChanged();
        }
        public async Task EnableAsync()
        {
            this.StorageGroupResponse.Status.Remove(StorageGroupStatus.Disable);
            await this.UpdateAsync();
            base.Snackbar.Add("啟用成功", MudBlazor.Severity.Info);
            base.StateHasChanged();
        }

        public async Task SaveChangeAsync()
        {
            if (this.IsEditMode)
            {
                var result = await this.UpdateAsync();
                base.MudDialog.Close(result);
            }
            else if (base.StateContainer.StorageProviderResponse != null)
            {
                var result = await base.HBKStorageApi.StoragegroupsPOSTAsync(base.StateContainer.StorageProviderResponse.Storage_provider_id, new StorageGroupAddRequest()
                {
                    Clear_mode = this.StorageGroupResponse.Clear_mode,
                    Clear_policy = new ClearPolicyRequest()
                    {
                        Rule = this.StorageGroupResponse.Clear_policy.Rule
                    },
                    Download_priority = this.StorageGroupResponse.Download_priority,
                    Name = this.StorageGroupResponse.Name,
                    Status = this.StorageGroupResponse.Status,
                    Sync_mode = this.StorageGroupResponse.Sync_mode,
                    Sync_policy = new SyncPolicyRequest()
                    {
                        Rule = this.StorageGroupResponse.Sync_policy.Rule
                    },
                    Type = this.StorageGroupResponse.Type,
                    Upload_priority = this.StorageGroupResponse.Upload_priority
                });
                base.MudDialog.Close(result);
            }
        }
        private Task<StorageGroupResponse> UpdateAsync()
        {
            return base.HBKStorageApi.StoragegroupsPUTAsync(this.StorageGroupResponse.Storage_group_id, new StorageGroupUpdateRequest()
            {
                Clear_mode = this.StorageGroupResponse.Clear_mode,
                Clear_policy = new ClearPolicyRequest()
                {
                    Rule = this.StorageGroupResponse.Clear_policy.Rule
                },
                Download_priority = this.StorageGroupResponse.Download_priority,
                Name = this.StorageGroupResponse.Name,
                Status = this.StorageGroupResponse.Status,
                Sync_mode = this.StorageGroupResponse.Sync_mode,
                Sync_policy = new SyncPolicyRequest()
                {
                    Rule = this.StorageGroupResponse.Sync_policy.Rule
                },
                Type = this.StorageGroupResponse.Type,
                Upload_priority = this.StorageGroupResponse.Upload_priority
            });
        }
    }
}
