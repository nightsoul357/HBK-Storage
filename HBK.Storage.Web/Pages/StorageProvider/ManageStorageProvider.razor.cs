using HBK.Storage.Web.Containers;
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
    public partial class ManageStorageProvider
    {
        private MudTable<StorageProviderResponse> _table;
        private string _searchString = null;
        [Inject]
        public HBKStorageApi HBKStorageApi { get; set; }
        [Inject]
        public HBKDialogService HBKDialogService { get; set; }
        [Inject]
        public StateContainer StateContainer { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
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

            var response = await this.HBKStorageApi.StorageprovidersGET2Async(filter, order, state.Page, state.PageSize);

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
            var result = await this.HBKDialogService.ShowBasicAsync("刪除", $"確定刪除 { storageProvider.Name } 嗎(此操作無法復原)?", "刪除", Color.Error);
            if (result.Cancelled)
            {
                return;
            }
            await this.HBKStorageApi.StorageprovidersDELETEAsync(storageProvider.Storage_provider_id);
            await _table.ReloadServerData();
        }

        public void SelectStorageProvider(StorageProviderResponse storageProvider)
        {
            this.StateContainer.StorageProvider = storageProvider;
            this.NavigationManager.NavigateTo("/storageGroup/manage", true);
        }

        public async Task ShowEditDialogAsync(StorageProviderResponse storageProvider)
        {
            var result = await this.HBKDialogService.ShowEditStorageProviderAsync(storageProvider);
            await _table.ReloadServerData();
        }

        public async Task ShowAddDialogAsync()
        {
            var result = await this.HBKDialogService.ShowAddStorageProviderAsync();
            await _table.ReloadServerData();
        }
    }
}