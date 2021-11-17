using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HBK.Storage.Dashboard.Pages.StorageProviders
{
    public partial class AddStorageProviderDialog : HBKDialogBase
    {
        /// <summary>
        /// 取得或設定名稱
        /// </summary>
        public string Name { get; set; } = String.Empty;
        
        public async Task SaveChangeAsync()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                base.Snackbar.Add("欄位為必填", Severity.Warning);
                return;
            }

            var storageProvider = await base.HBKStorageApi.StorageprovidersPOSTAsync(new DataSource.StorageProviderAddRequest()
            {
                Name = this.Name,
                Status = new List<StorageProviderStatus>()
            });

            base.MudDialog.Close(storageProvider);
        }
    }
}
