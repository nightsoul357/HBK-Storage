using HBK.Storage.Web.Containers;
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
    public partial class ManageStorageGroup
    {
        private MudTable<StorageGroupResponse> _table;
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
        public async Task<TableData<StorageGroupResponse>> ServerReloadAsync(TableState state)
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

            var response = await this.HBKStorageApi.StoragegroupsGET2Async(this.StateContainer.StorageProvider.Storage_provider_id, filter, order, state.Page, state.PageSize);

            return new TableData<StorageGroupResponse>()
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
        public void NavigateToManage(StorageGroupResponse storageGroup)
        {
            this.StateContainer.StorageGroup = storageGroup;
            this.NavigationManager.NavigateTo("/storage/manage");
        }
        public async Task ShowEditDialogAsync(StorageGroupResponse storageGroup)
        {
            await this.HBKDialogService.ShowEditStorageGroupAsync(storageGroup);
            await _table.ReloadServerData();
        }
        public async Task ShowDeleteDialogAsync(StorageGroupResponse storageGroup)
        {
            var result = await this.HBKDialogService.ShowBasicAsync("刪除", $"確定刪除 { storageGroup.Name } 嗎(此操作無法復原)?", "刪除", Color.Error);
            if (result.Cancelled)
            {
                return;
            }
            await this.HBKStorageApi.StoragegroupsDELETEAsync(storageGroup.Storage_group_id);
            await _table.ReloadServerData();
        }
        public async Task ShowAddDialogAsync()
        {
            await this.HBKDialogService.ShowAddStorageGroupAsync();
            await _table.ReloadServerData();
        }
    }
}
