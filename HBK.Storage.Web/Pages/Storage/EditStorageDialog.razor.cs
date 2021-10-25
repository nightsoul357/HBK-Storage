using HBK.Storage.Web.Containers;
using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.Pages.Storage.Credential;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage
{
    public partial class EditStorageDialog
    {
        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }
        public string Name { get; set; }
        public long SizeLimit { get; set; }
        public StorageType StorageType { get; set; }
        [Parameter]
        public StorageResponse EditStorage { get; set; }
        [Inject]
        public HBKStorageApi HBKStorageApi { get; set; }
        [Inject]
        public StateContainer StateContainer { get; set; }
        public ICredentialsComponent CredentialsComponent { get; set; }
        public bool IsEditMode
        {
            get
            {
                return this.EditStorage != null;
            }
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (this.EditStorage != null)
            {
                this.Name = this.EditStorage.Name;
                this.SizeLimit = this.EditStorage.Size_limit;
                this.StorageType = this.EditStorage.Type;
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
                await this.HBKStorageApi.StoragesPUTAsync(this.EditStorage.Storage_id, new StorageUpdateRequest()
                {
                    Credentials = this.CredentialsComponent.Credential,
                    Name = this.Name,
                    Size_limit = this.SizeLimit,
                    Status = this.EditStorage.Status,
                    Type = this.StorageType
                });
                this.MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                await this.HBKStorageApi.StoragesPOSTAsync(this.StateContainer.StorageGroup.Storage_group_id, new StorageAddRequest()
                {
                    Credentials = this.CredentialsComponent.Credential,
                    Name = this.Name,
                    Size_limit = this.SizeLimit,
                    Status = new List<StorageStatus>(),
                    Type = this.StorageType
                });
                this.MudDialog.Close(DialogResult.Ok(true));
            }
        }
        public async Task Disable()
        {
            this.EditStorage.Status.Add(StorageStatus.Disable);
            await this.HBKStorageApi.StoragesPUTAsync(this.EditStorage.Storage_id, new StorageUpdateRequest()
            {
                Credentials = this.CredentialsComponent.Credential,
                Name = this.EditStorage.Name,
                Size_limit = this.EditStorage.Size_limit,
                Status = this.EditStorage.Status,
                Type = this.EditStorage.Type
            });
            this.MudDialog.Close(DialogResult.Ok(true));
        }
        public async Task Enable()
        {
            this.EditStorage.Status.Remove(StorageStatus.Disable);
            await this.HBKStorageApi.StoragesPUTAsync(this.EditStorage.Storage_id, new StorageUpdateRequest()
            {
                Credentials = this.CredentialsComponent.Credential,
                Name = this.EditStorage.Name,
                Size_limit = this.EditStorage.Size_limit,
                Status = this.EditStorage.Status,
                Type = this.EditStorage.Type
            });
            this.MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
