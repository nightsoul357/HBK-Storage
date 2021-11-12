using BlazorDownloadFile;
using HBK.Storage.Web.Containers;
using HBK.Storage.Web.DataAnnotations;
using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.Features;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace HBK.Storage.Web.Pages.FileEntity
{
    [StateValidation(IsStorageProviderValid = true)]
    public partial class ManageFileEntity : PageBase<ManageFileEntity>
    {
        private MudTable<FileEntityResponse> _table;
        private string _searchString = null;
        private int[] _timerInterval = { 2000, 4000, 8000, 16000, 32000, 64000 };
        private int _timerIntervalIndex = 0;
        private bool _isDisplayChildFileEntity = false;
        public bool IsDisplayChildFileEntity
        {
            get
            {
                return _isDisplayChildFileEntity;
            }
            set
            {
                _isDisplayChildFileEntity = value;
                _table.ReloadServerData();
            }
        }
        public Timer Timer { get; set; }
        [Inject]
        public IBlazorDownloadFileService DownloadFileService { get; set; }
        /// <summary>
        /// Here we simulate getting the paged, filtered and ordered data from the server
        /// </summary>
        public async Task<TableData<FileEntityResponse>> ServerReloadAsync(TableState state)
        {
            string filter = null;
            string order = null;

            if (!this.IsDisplayChildFileEntity)
            {
                filter += $"parent_file_entity_id eq null and ";
            }
            if (!string.IsNullOrEmpty(_searchString))
            {
                filter += $"(contains(name,'{_searchString}') or contains(cast(file_entity_id, 'Edm.String'),'{_searchString}') or file_entity_tag/any(t:contains(t/value,'{_searchString}')) or contains(mime_type,'{_searchString}'))";
            }
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                order = $"{state.SortLabel} {(state.SortDirection == SortDirection.Ascending ? "asc" : "desc")}";
            }

            if (filter != null)
            {
                filter = filter.Trim("and ".ToArray());
            }

            var response = await this.HBKStorageApi.FileentitiesGET2Async(this.StateContainer.StorageProvider.Storage_provider_id, filter, order, state.Page * state.PageSize, state.PageSize);

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

        public async Task ShowPublishAccessTokenDialogAsync(FileEntityResponse fileEntity)
        {
            await this.HBKDialogService.ShowPublishAccessTokenAsync(fileEntity);
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
            foreach (IBrowserFile file in e.GetMultipleFiles(int.MaxValue))
            {
                List<string> tags = new List<string>();
                if (file.ContentType.StartsWith("image"))
                {
                    tags.Add("Require-Compress-Image");
                }
                var fileName = Path.Combine(Directory.GetCurrentDirectory(), Path.GetTempFileName());

                try
                {
                    await using FileStream fs = new FileStream(fileName, FileMode.Create);
                    await file.OpenReadStream(long.MaxValue).CopyToAsync(fs);
                    fs.Seek(0, SeekOrigin.Begin);
                    await this.HBKStorageApi.FileentitiesPOSTAsync(this.StateContainer.StorageProvider.Storage_provider_id, file.Name, null, null, tags,
                        file.ContentType,
                        new FileParameter(fs));
                    await _table.ReloadServerData();
                    base.Snackbar.Add("上傳完成", Severity.Info);
                }
                catch (Exception ex)
                {
                    base.Snackbar.Add($"上傳失敗：{ex.Message}", Severity.Error);
                }
                finally
                {
                    File.Delete(fileName);
                }
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
