using HBK.Storage.Dashboard.DataSource;
using MudBlazor;

namespace HBK.Storage.Dashboard.Pages.AuthorizeKey
{
    public partial class AuthorizeKeyInformation : HBKPageBase
    {
        private MudTable<AuthorizeKeyResponse> _table;
        private string? _searchString = null;

        /// <summary>
        /// Here we simulate getting the paged, filtered and ordered data from the server
        /// </summary>
        public async Task<TableData<AuthorizeKeyResponse>> ServerReloadAsync(TableState state)
        {
            string? filter = null;
            string? order = null;

            if (!string.IsNullOrEmpty(_searchString))
            {
                filter = $"contains(name,'{_searchString}')";
            }
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                order = $"{state.SortLabel} {(state.SortDirection == SortDirection.Ascending ? "asc" : "desc")}";
            }

            var response = await base.HBKStorageApi.AuthorizekeyGET2Async(filter, order, state.Page * state.PageSize, state.PageSize);
            return new TableData<AuthorizeKeyResponse>()
            {
                Items = response.Value,
                TotalItems = response.OdataCount
            };
        }
        public async Task OnSearchAsync(string text)
        {
            _searchString = text;
            await _table.ReloadServerData();
        }
        public async Task ShowInformationDialogAsync(AuthorizeKeyResponse authorizeKey)
        {
            var result = await base.DialogService.ShowEditAuthorizeKeyAsync(authorizeKey);
            await _table.ReloadServerData();
        }

        public async Task ShowDeleteDialogAsync(AuthorizeKeyResponse authorizeKey)
        {
            if (authorizeKey.Authorize_key_id == this.StateContainer.GetSignInKey().Authorize_key_id)
            {
                base.Snackbar.Add("無法刪除使用中的驗證金鑰", Severity.Error);
                return;
            }
            var result = await base.DialogService.ShowBasicAsync("刪除", $"確定刪除 { authorizeKey.Name } 嗎(此操作無法復原)?", "刪除", Color.Error);
            if (result.Cancelled)
            {
                return;
            }
            await base.HBKStorageApi.AuthorizekeyDELETEAsync(authorizeKey.Authorize_key_id);
            await _table.ReloadServerData();
        }
        public async Task ShowAddDialogAsync()
        {
            var result = await base.DialogService.ShowAddAuthorizeKeyAsync();
            await _table.ReloadServerData();
        }
    }
}
