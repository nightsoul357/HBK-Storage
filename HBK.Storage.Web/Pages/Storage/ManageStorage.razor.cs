using HBK.Storage.Web.Containers;
using HBK.Storage.Web.DataAnnotations;
using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.Features;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage
{
    [StateValidation(IsStorageGroupValid = true, IsStorageProviderValid = true)]
    public partial class ManageStorage : PageBase<ManageStorage>
    {
        private MudTable<StorageExtendPropertyResponse> _table;
        private string _searchString = null;
        /// <summary>
        /// Here we simulate getting the paged, filtered and ordered data from the server
        /// </summary>
        public async Task<TableData<StorageExtendPropertyResponse>> ServerReloadAsync(TableState state)
        {
            string filter = null;
            string order = null;

            if (!string.IsNullOrEmpty(_searchString))
            {
                filter = $"contains(name,'{_searchString}')";
            }
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                order = $"{state.SortLabel} {(state.SortDirection == SortDirection.Ascending ? "asc" : "desc")}";
            }

            var response = await this.HBKStorageApi.StorageextendpropertiesAsync(this.StateContainer.StorageGroup.Storage_group_id, filter, order, state.Page * state.PageSize, state.PageSize);

            return new TableData<StorageExtendPropertyResponse>()
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

        public async Task ShowEditDialogAsync(StorageExtendPropertyResponse storage)
        {
            await this.HBKDialogService.ShowEditStorageAsync(storage);
            await _table.ReloadServerData();
        }
        public async Task ShowDeleteDialogAsync(StorageExtendPropertyResponse storage)
        {
            var result = await this.HBKDialogService.ShowBasicAsync("刪除", $"確定刪除 { storage.Name } 嗎(此操作無法復原)?", "刪除", Color.Error);
            if (result.Cancelled)
            {
                return;
            }
            await this.HBKStorageApi.StoragesDELETEAsync(storage.Storage_id);
            await _table.ReloadServerData();
        }
        public void GoBack()
        {
            this.NavigationManager.NavigateTo("/storageGroup/manage");
        }
        public async Task ShowAddDialogAsync()
        {
            await this.HBKDialogService.ShowAddStorageAsync();
            await _table.ReloadServerData();
        }
    }
}
