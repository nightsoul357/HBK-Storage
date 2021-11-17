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
            if (this.StorageGroupId != null)
            {
                this.StorageGroupResponse = await base.HBKStorageApi.StoragegroupsGETAsync(Guid.Parse(this.StorageGroupId));
                this.StorageExtendPropertyResponses = (await base.HBKStorageApi.StorageextendpropertiesAsync(this.StorageGroupResponse.Storage_group_id, null, null, null, null)).Value.ToList();
            }
            await base.OnParametersSetAsync();
        }
    }
}
