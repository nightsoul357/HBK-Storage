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

namespace HBK.Storage.Web.Pages.StorageProvider
{
    public partial class ManageStorageProvider : PageBase<ManageStorageProvider>
    {
        private MudTable<StorageProviderResponse> _table;
        private string _searchString = null;
        
        /// <summary>
        /// Here we simulate getting the paged, filtered and ordered data from the server
        /// </summary>
        public async Task<TableData<StorageProviderResponse>> ServerReloadAsync(TableState state)
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

            var response = await base.HBKStorageApi.StorageprovidersGET2Async(filter, order, state.Page * state.PageSize, state.PageSize);

            return new TableData<StorageProviderResponse>()
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

        public async Task ShowDeleteDialogAsync(StorageProviderResponse storageProvider)
        {
            var result = await base.HBKDialogService.ShowBasicAsync("刪除", $"確定刪除 { storageProvider.Name } 嗎(此操作無法復原)?", "刪除", Color.Error);
            if (result.Cancelled)
            {
                return;
            }
            await base.HBKStorageApi.StorageprovidersDELETEAsync(storageProvider.Storage_provider_id);
            await _table.ReloadServerData();
            if (base.StateContainer.StorageProvider != null && storageProvider.Storage_provider_id == base.StateContainer.StorageProvider.Storage_provider_id)
            {
                base.StateContainer.StorageProvider = null;
                base.NavigationManager.NavigateTo("/");
            }
        }

        public void SelectStorageProvider(StorageProviderResponse storageProvider)
        {
            base.StateContainer.StorageProvider = storageProvider;
            base.NavigationManager.NavigateTo("/storageGroup/manage", true);
        }

        public async Task ShowEditDialogAsync(StorageProviderResponse storageProvider)
        {
            var result = await base.HBKDialogService.ShowEditStorageProviderAsync(storageProvider);
            await _table.ReloadServerData();
        }

        public async Task ShowAddDialogAsync()
        {
            var result = await base.HBKDialogService.ShowAddStorageProviderAsync();
            await _table.ReloadServerData();
        }
    }
}