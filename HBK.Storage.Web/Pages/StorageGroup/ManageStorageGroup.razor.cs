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

namespace HBK.Storage.Web.Pages.StorageGroup
{
    [StateValidation(IsStorageProviderValid = true)]
    public partial class ManageStorageGroup : PageBase<ManageStorageGroup>
    {
        private MudTable<StorageGroupExtendPropertyResponse> _table;
        private string _searchString = null;
        /// <summary>
        /// Here we simulate getting the paged, filtered and ordered data from the server
        /// </summary>
        public async Task<TableData<StorageGroupExtendPropertyResponse>> ServerReloadAsync(TableState state)
        {
            string filter = null;
            string order = null;

            if (!string.IsNullOrEmpty(_searchString))
            {
                filter = $"contains(storage_group/name,'{_searchString}')";
            }
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                order = $"{state.SortLabel} {(state.SortDirection == SortDirection.Ascending ? "asc" : "desc")}";
            }

            var response = await base.HBKStorageApi.StoragegroupextendpropertiesAsync(base.StateContainer.StorageProvider.Storage_provider_id, filter, order, state.Page * state.PageSize, state.PageSize);

            return new TableData<StorageGroupExtendPropertyResponse>()
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
        public void NavigateToManage(StorageGroupExtendPropertyResponse storageGroup)
        {
            base.StateContainer.StorageGroup = storageGroup;
            base.NavigationManager.NavigateTo("/storage/manage");
        }
        public async Task ShowEditDialogAsync(StorageGroupExtendPropertyResponse storageGroup)
        {
            await base.HBKDialogService.ShowEditStorageGroupAsync(storageGroup);
            await _table.ReloadServerData();
        }
        public async Task ShowDeleteDialogAsync(StorageGroupExtendPropertyResponse storageGroup)
        {
            var result = await base.HBKDialogService.ShowBasicAsync("刪除", $"確定刪除 { storageGroup.Name } 嗎(此操作無法復原)?", "刪除", Color.Error);
            if (result.Cancelled)
            {
                return;
            }
            await base.HBKStorageApi.StoragegroupsDELETEAsync(storageGroup.Storage_group_id);
            await _table.ReloadServerData();
        }
        public async Task ShowAddDialogAsync()
        {
            await base.HBKDialogService.ShowAddStorageGroupAsync();
            await _table.ReloadServerData();
        }
    }
}
