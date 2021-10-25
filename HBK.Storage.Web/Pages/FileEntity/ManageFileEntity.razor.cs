using BlazorDownloadFile;
using HBK.Storage.Web.Containers;
using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.Features;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace HBK.Storage.Web.Pages.FileEntity
{
    public partial class ManageFileEntity
    {
        private MudTable<FileEntityResponse> _table;
        private string _searchString = null;
        private int[] _timerInterval = { 2000, 4000, 8000, 16000, 32000, 64000 };
        private int _timerIntervalIndex = 0;
        public Timer Timer { get; set; }
        [Inject]
        public HBKStorageApi HBKStorageApi { get; set; }
        [Inject]
        public HBKDialogService HBKDialogService { get; set; }
        [Inject]
        public StateContainer StateContainer { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IBlazorDownloadFileService DownloadFileService { get; set; }
        /// <summary>
        /// Here we simulate getting the paged, filtered and ordered data from the server
        /// </summary>
        public async Task<TableData<FileEntityResponse>> ServerReloadAsync(TableState state)
        {
            string filter = "parent_file_entity_id eq null";
            string order = null;

            if (!string.IsNullOrEmpty(_searchString))
            {
                filter += $"& contains(name,'{_searchString}')";
            }
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                order = $"{state.SortLabel} {(state.SortDirection == SortDirection.Ascending ? "asc" : "desc")}";
            }

            var response = await this.HBKStorageApi.FileentitiesGET2Async(this.StateContainer.StorageProvider.Storage_provider_id, filter, order, state.Page, state.PageSize);

            return new TableData<FileEntityResponse>()
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

        public async Task ShowInformationDialogAsync(FileEntityResponse fileEntity)
        {
            await this.HBKDialogService.ShowFileEntityInformationAsync(fileEntity);
        }

        public async Task DownloadFileAsync(FileEntityResponse fileEntity)
        {
            var response = await this.HBKStorageApi.FileentitiesGETAsync(fileEntity.File_entity_id, null);
            await this.DownloadFileService.DownloadFile(fileEntity.Name, response.Stream, fileEntity.Mime_type);
        }
        public async Task ShowDeleteDialogAsync(FileEntityResponse fileEntity)
        {
            var result = await this.HBKDialogService.ShowBasicAsync("刪除", $"確定刪除 { fileEntity.Name } 嗎(此操作無法復原)?", "刪除", Color.Error);
            if (result.Cancelled)
            {
                return;
            }
            await this.HBKStorageApi.FileentitiesDELETEAsync(fileEntity.File_entity_id);
            await _table.ReloadServerData();
            this.StartFetchDataTimer();
        }
        public async Task UploadFilesAsync(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles())
            {
                await this.HBKStorageApi.FileentitiesPOSTAsync(this.StateContainer.StorageProvider.Storage_provider_id, file.Name, null, null, null,
                    file.ContentType,
                    new FileParameter(file.OpenReadStream(long.MaxValue)));
                await _table.ReloadServerData();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.Timer = new Timer(_timerInterval[_timerIntervalIndex]);
            this.Timer.Elapsed += this.Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Timer.Stop();
            _table.ReloadServerData();
            _timerIntervalIndex++;
            if (_timerIntervalIndex < _timerInterval.Length)
            {
                this.Timer.Interval = _timerInterval[_timerIntervalIndex];
                this.Timer.Start();
            }
        }
        private void StartFetchDataTimer()
        {
            this.Timer.Stop();
            _timerIntervalIndex = 0;
            this.Timer.Interval = _timerInterval[_timerIntervalIndex];
            this.Timer.Start();
        }
    }
}
