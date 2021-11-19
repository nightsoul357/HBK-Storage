using HBK.Storage.Dashboard.DataSource;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HBK.Storage.Dashboard.Pages.FileEntities
{
    public partial class PublishAccessTokenDialog
    {
        /// <summary>
        /// 取得或設定 Dialog 中使用的字定義屬性(解決 Dialog 無法反白選取的問題)
        /// <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/1186"/>
        /// </summary>
        public Dictionary<string, object> DialogAttributes { get; set; } =
        new()
        {
            { "tabindex", "10" },
        };

        [Parameter]
        public FileEntityResponse FileEntity { get; set; }

        public FileAccessTokenType FileAccessTokenType { get; set; }
        public string FileLink { get; set; }

        public string HandlerIndicate { get; set; } = string.Empty;
        public int AccessTimesLimit { get; set; } = 10;
        public int ExpireAfterMinute { get; set; } = 60;

        public async Task PublishAsync()
        {
            PostFileAccessTokenResponse result = null;
            if (this.FileAccessTokenType == FileAccessTokenType.Normal)
            {
                result = await this.HBKStorageApi.FileaccesstokensPOSTAsync(base.StateContainer.StorageProviderResponse.Storage_provider_id, new PostFileAccessTokenRequest()
                {
                    Access_times_limit = this.AccessTimesLimit,
                    Expire_after_minutes = this.ExpireAfterMinute,
                    File_access_token_type = FileAccessTokenType.Normal,
                    File_entity_id = this.FileEntity.File_entity_id,
                    Handler_indicate = this.HandlerIndicate
                });
            }
            else if (this.FileAccessTokenType == FileAccessTokenType.Normal_no_limit)
            {
                result = await this.HBKStorageApi.FileaccesstokensPOSTAsync(base.StateContainer.StorageProviderResponse.Storage_provider_id, new PostFileAccessTokenRequest()
                {
                    File_access_token_type = FileAccessTokenType.Normal_no_limit,
                    File_entity_id = this.FileEntity.File_entity_id,
                    Handler_indicate = this.HandlerIndicate,
                    Expire_after_minutes = this.ExpireAfterMinute
                });
            }
            if (result != null)
            {
                this.FileLink = this.HBKStorageApi.BaseUrl + $"docs?esic={result.Token}";
            }
            base.StateHasChanged();
        }

        public async Task CopyLinkToClipboardAsync()
        {
            await base.ClipboardService.WriteTextAsync(this.FileLink);
            this.Snackbar.Add("已複製到剪貼簿", Severity.Success);
        }

    }
}
