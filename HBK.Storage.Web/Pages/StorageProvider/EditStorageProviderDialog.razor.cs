using HBK.Storage.Web.DataSource;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.StorageProvider
{
    public partial class EditStorageProviderDialog
    {
        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }
        public string Name { get; set; }
        [Parameter]
        public StorageProviderResponse EditStorageProvider { get; set; }
        [Inject]
        public HBKStorageApi HBKStorageApi { get; set; }
        public bool IsEditMode
        {
            get
            {
                return this.EditStorageProvider != null;
            }
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (this.EditStorageProvider != null)
            {
                this.Name = this.EditStorageProvider.Name;
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
                await this.HBKStorageApi.StorageprovidersPUTAsync(this.EditStorageProvider.Storage_provider_id, new StorageProviderUpdateRequest()
                {
                    Name = this.Name,
                    Status = this.EditStorageProvider.Status
                });
                this.MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                await this.HBKStorageApi.StorageprovidersPOSTAsync(new StorageProviderAddRequest()
                {
                    Name = this.Name,
                    Status = new List<StorageProviderStatus>()
                });
                this.MudDialog.Close(DialogResult.Ok(true));
            }
        }
        public async Task Disable()
        {
            this.EditStorageProvider.Status.Add(StorageProviderStatus.Disable);
            await this.HBKStorageApi.StorageprovidersPUTAsync(this.EditStorageProvider.Storage_provider_id, new StorageProviderUpdateRequest()
            {
                Name = this.EditStorageProvider.Name,
                Status = this.EditStorageProvider.Status
            });
            this.MudDialog.Close(DialogResult.Ok(true));
        }

        public async Task Enable()
        {
            this.EditStorageProvider.Status.Remove(StorageProviderStatus.Disable);
            await this.HBKStorageApi.StorageprovidersPUTAsync(this.EditStorageProvider.Storage_provider_id, new StorageProviderUpdateRequest()
            {
                Name = this.EditStorageProvider.Name,
                Status = this.EditStorageProvider.Status
            });
            this.MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
