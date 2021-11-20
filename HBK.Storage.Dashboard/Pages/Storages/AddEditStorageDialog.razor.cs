using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.DataSource.Models;
using HBK.Storage.Dashboard.Helpers;
using HBK.Storage.Dashboard.Pages.Storages.Credentials;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace HBK.Storage.Dashboard.Pages.Storages
{
    public partial class AddEditStorageDialog : HBKDialogBase
    {
        [Parameter]
        public StorageResponse StorageResponse { get; set; } = new StorageResponse()
        {
            Status = new List<StorageStatus>()
        };
        [Parameter]
        public Guid? StorageGroupId { get; set; }
        [Parameter]
        public bool IsEditMode { get; set; } = false;

        public ICredentialsComponent? CredentialsComponent { get; set; }

        private StorageCredentialsBase ConvertToCredential(string source, StorageType storageType)
        {
            switch (storageType)
            {
                case StorageType.Local:
                    return JsonConvertHelper.DeserializeObjectWithDefault(source, new LocalStorageCredentials());
                case StorageType.Amazon_s3:
                    return JsonConvertHelper.DeserializeObjectWithDefault(source, new AmazonS3StorageCredentials());
                case StorageType.Google_drive:
                    return JsonConvertHelper.DeserializeObjectWithDefault(source, new GoogleDriveCredentials());
                case StorageType.Mega:
                    return JsonConvertHelper.DeserializeObjectWithDefault(source, new MegaStorageCredentials());
                case StorageType.Ftp:
                    return JsonConvertHelper.DeserializeObjectWithDefault(source, new FTPStorageCredentials());
                case StorageType.Web_dav:
                    return JsonConvertHelper.DeserializeObjectWithDefault(source, new WebDAVStorageCredentials());
            }

            throw new NotImplementedException();
        }

        public async Task SaveChangeAsync()
        {
            if (!this.StorageGroupId.HasValue || CredentialsComponent == null)
            {
                return;
            }

            if (this.IsEditMode)
            {
                var result = await this.UpdateAsync();
                base.MudDialog.Close(result);
            }
            else
            {
                var result = await base.HBKStorageApi.StoragesPOSTAsync(this.StorageGroupId.Value, new StorageAddRequest()
                {
                    Credentials = this.CredentialsComponent.Credential,
                    Name = this.StorageResponse.Name,
                    Size_limit = this.StorageResponse.Size_limit,
                    Status = this.StorageResponse.Status,
                    Type = this.StorageResponse.Type
                });
                base.MudDialog.Close(result);
            }
        }
        public async Task EnableAsync()
        {
            if (this.IsEditMode)
            {
                this.StorageResponse.Status.Remove(StorageStatus.Disable);
                var result = await this.UpdateAsync();
                base.Snackbar.Add("啟用成功", MudBlazor.Severity.Info);
            }
        }
        public async Task DisableAsync()
        {
            if (this.IsEditMode)
            {
                this.StorageResponse.Status.Add(StorageStatus.Disable);
                var result = await this.UpdateAsync();
                base.Snackbar.Add("停用成功", MudBlazor.Severity.Info);
            }
        }
        private async Task<StorageResponse> UpdateAsync()
        {
            var result = await base.HBKStorageApi.StoragesPUTAsync(this.StorageResponse.Storage_id, new StorageUpdateRequest()
            {
                Credentials = this.CredentialsComponent.Credential,
                Name = this.StorageResponse.Name,
                Size_limit = this.StorageResponse.Size_limit,
                Status = this.StorageResponse.Status,
                Type = this.StorageResponse.Type
            });
            return result;
        }
        private void StorageTypeChnage()
        {
            this.StorageResponse.Credentials = String.Empty;
        }
    }
}
