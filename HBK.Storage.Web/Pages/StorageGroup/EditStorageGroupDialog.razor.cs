using HBK.Storage.Web.Containers;
using HBK.Storage.Web.DataSource;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.StorageGroup
{
    public partial class EditStorageGroupDialog
    {
        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }
        [Inject]
        public HBKStorageApi HBKStorageApi { get; set; }
        [Inject]
        public StateContainer StateContainer { get; set; }
        [Parameter]
        public StorageGroupExtendPropertyResponse EditStorageGroup { get; set; }
        public int UploadPriority { get; set; } = 1;
        public int DownloadPriority { get; set; } = 1;
        public string Name { get; set; }
        public StorageType StorageType { get; set; }
        public SyncMode SyncMode { get; set; }
        public ClearMode ClearMode { get; set; }
        public string ClearPolicyRule { get; set; }
        public string SyncPolicyRule { get; set; }
        public bool IsEditMode
        {
            get
            {
                return this.EditStorageGroup != null;
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (this.EditStorageGroup != null)
            {
                this.Name = this.EditStorageGroup.Name;
                this.StorageType = this.EditStorageGroup.Type;
                this.SyncMode = this.EditStorageGroup.Sync_mode;
                this.UploadPriority = this.EditStorageGroup.Upload_priority;
                this.DownloadPriority = this.EditStorageGroup.Download_priority;
                this.ClearMode = this.EditStorageGroup.Clear_mode;
                if (this.EditStorageGroup.Sync_policy != null)
                {
                    this.SyncPolicyRule = this.EditStorageGroup.Sync_policy.Rule;
                }
                if (this.EditStorageGroup.Clear_policy != null)
                {
                    this.ClearPolicyRule = this.EditStorageGroup.Clear_policy.Rule;
                }
            }
        }
        public void Cancel()
        {
            this.MudDialog.Cancel();
        }
        public async Task SaveChangeAsync()
        {
            if (this.IsEditMode)
            {
                _ = await this.HBKStorageApi.StoragegroupsPUTAsync(this.EditStorageGroup.Storage_group_id, new StorageGroupUpdateRequest()
                {
                    Download_priority = this.DownloadPriority,
                    Name = this.Name,
                    Status = this.EditStorageGroup.Status,
                    Sync_mode = this.SyncMode,
                    Sync_policy = new SyncPolicyRequest()
                    {
                        Rule = this.SyncPolicyRule
                    },
                    Type = this.StorageType,
                    Upload_priority = this.UploadPriority,
                    Clear_mode = this.ClearMode,
                    Clear_policy = new ClearPolicyRequest()
                    {
                        Rule = this.ClearPolicyRule
                    }
                });
                this.MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                _ = await this.HBKStorageApi.StoragegroupsPOSTAsync(this.StateContainer.StorageProvider.Storage_provider_id, new StorageGroupAddRequest()
                {
                    Download_priority = this.DownloadPriority,
                    Name = this.Name,
                    Status = new List<StorageGroupStatus>(),
                    Sync_mode = this.SyncMode,
                    Sync_policy = new SyncPolicyRequest()
                    {
                        Rule = this.SyncPolicyRule
                    },
                    Clear_mode = this.ClearMode,
                    Clear_policy = new ClearPolicyRequest()
                    {
                        Rule = this.ClearPolicyRule
                    },
                    Type = this.StorageType,
                    Upload_priority = this.UploadPriority
                });
                this.MudDialog.Close(DialogResult.Ok(true));
            }
        }

        public async Task Enable()
        {
            this.EditStorageGroup.Status.Remove(StorageGroupStatus.Disable);
            _ = await this.HBKStorageApi.StoragegroupsPUTAsync(this.EditStorageGroup.Storage_group_id, new StorageGroupUpdateRequest()
            {
                Download_priority = this.EditStorageGroup.Download_priority,
                Name = this.EditStorageGroup.Name,
                Status = this.EditStorageGroup.Status,
                Sync_mode = this.EditStorageGroup.Sync_mode,
                Sync_policy = new SyncPolicyRequest()
                {
                    Rule = this.EditStorageGroup.Sync_policy?.Rule
                },
                Clear_mode = this.EditStorageGroup.Clear_mode,
                Clear_policy = new ClearPolicyRequest() 
                {
                    Rule = this.EditStorageGroup.Clear_policy?.Rule
                },
                Type = this.EditStorageGroup.Type,
                Upload_priority = this.EditStorageGroup.Upload_priority
            });
            this.MudDialog.Close(DialogResult.Ok(true));
        }

        public async Task Disable()
        {
            this.EditStorageGroup.Status.Add(StorageGroupStatus.Disable);
            _ = await this.HBKStorageApi.StoragegroupsPUTAsync(this.EditStorageGroup.Storage_group_id, new StorageGroupUpdateRequest()
            {
                Download_priority = this.EditStorageGroup.Download_priority,
                Name = this.EditStorageGroup.Name,
                Status = this.EditStorageGroup.Status,
                Sync_mode = this.EditStorageGroup.Sync_mode,
                Sync_policy = new SyncPolicyRequest()
                {
                    Rule = this.EditStorageGroup.Sync_policy?.Rule
                },
                Clear_mode = this.EditStorageGroup.Clear_mode,
                Clear_policy = new ClearPolicyRequest() 
                {
                    Rule = this.EditStorageGroup.Clear_policy?.Rule
                },
                Type = this.EditStorageGroup.Type,
                Upload_priority = this.EditStorageGroup.Upload_priority
            });
            this.MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
