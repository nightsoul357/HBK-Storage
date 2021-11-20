using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Models;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace HBK.Storage.Dashboard.Pages.FileEntities
{
    public partial class FileEntityInformation : HBKPageBase
    {
        private MudTable<FileEntityResponse> _table;
        private string? _searchString = null;
        private List<UploadFileTask> UploadFileTask = new List<UploadFileTask>();
        private System.Timers.Timer _timer = new System.Timers.Timer(1000);
        private bool _isUploading = false;
        private UploadFileConfig _uploadFileConfig = new UploadFileConfig();

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
        public async Task ShowEditUploadFileConfigDialogAsync()
        {
            var result = await base.DialogService.ShowEditUploadFileConfigAsync(_uploadFileConfig);
            if (!result.Cancelled)
            {
                _uploadFileConfig = result.Data;
            }
        }
        public Task ShowPublishAccessTokenDialogAsync(FileEntityResponse fileEntity)
        {
            return base.DialogService.ShowPublishFileAccessTokenDialogAsync(fileEntity);
        }
        public async Task DownloadFileAsync(FileEntityResponse fileEntity)
        {
            var response = await this.HBKStorageApi.FileentitiesGETAsync(fileEntity.File_entity_id, null, string.Empty);
            await this.DownloadFileService.DownloadFile(fileEntity.Name, response.Stream, fileEntity.Mime_type);
        }
        public async Task ShowDeleteDialogAsync(FileEntityResponse fileEntity)
        {
            var result = await this.DialogService.ShowBasicAsync("刪除", $"確定刪除 { fileEntity.Name } 嗎(此操作無法復原)?", "刪除", Color.Error);
            if (result.Cancelled)
            {
                return;
            }
            await this.HBKStorageApi.FileentitiesDELETEAsync(fileEntity.File_entity_id);
            await _table.ReloadServerData();
        }

        public void ClearCompleteTask()
        {
            this.UploadFileTask.RemoveAll(x => x.Status == Enums.UploadFileTaskStatusEnum.Complete);
            base.StateHasChanged();
        }

        public void ClearAllTask()
        {
            this.UploadFileTask.RemoveAll(x => x.Status != Enums.UploadFileTaskStatusEnum.Uploading);
            base.StateHasChanged();
        }

        public void RetryAllFailTask()
        {
            this.UploadFileTask.Where(x => x.Status == Enums.UploadFileTaskStatusEnum.Terminate).ToList()
                .ForEach(x => 
                {
                    x.Status = Enums.UploadFileTaskStatusEnum.Pending;
                    x.Exception = null;
                    x.CompleteDateTime = null;
                });

            base.StateHasChanged();
            if (!_isUploading && !_timer.Enabled)
            {
                _timer.Start();
            }
        }

        public void UploadFiles(InputFileChangeEventArgs e)
        {
            foreach (IBrowserFile file in e.GetMultipleFiles(int.MaxValue))
            {
                this.UploadFileTask.Add(new UploadFileTask()
                {
                    File = file,
                    Progress = 0,
                    CreateDateTime = DateTime.Now,
                    Status = Enums.UploadFileTaskStatusEnum.Pending,
                    UploadFileConfig = new UploadFileConfig()
                    {
                        CryptoMode = _uploadFileConfig.CryptoMode,
                        Tags = _uploadFileConfig.Tags.Select(x => x).ToList()
                    }
                });
            }

            if (!_isUploading && !_timer.Enabled)
            {
                _timer.Start();
            }
        }

        public async Task ShowFileEntityDetailDialogAsync(FileEntityResponse fileEntityResponse)
        {
            await base.DialogService.ShowFileDetailDialogAsync(fileEntityResponse);
        }

        /// <summary>
        /// Here we simulate getting the paged, filtered and ordered data from the server
        /// </summary>
        public async Task<TableData<FileEntityResponse>> ServerReloadAsync(TableState state)
        {
            try
            {
                if (base.StateContainer.StorageProviderResponse == null)
                {
                    return new TableData<FileEntityResponse>()
                    {
                        TotalItems = 0
                    };
                }

                string? filter = null;
                string? order = null;

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

                var response = await this.HBKStorageApi.FileentitiesGET2Async(base.StateContainer.StorageProviderResponse.Storage_provider_id, filter, order, state.Page * state.PageSize, state.PageSize);

                return new TableData<FileEntityResponse>()
                {
                    Items = response.Value,
                    TotalItems = response.OdataCount
                };
            }
            catch (Exception ex)
            {
                base.Snackbar.Add("發生未預期的錯誤", Severity.Error);
                return new TableData<FileEntityResponse>()
                {
                    TotalItems = 0
                };
            }
        }

        public async Task OnSearchAsync(string text)
        {
            _searchString = text;
            await _table.ReloadServerData();
        }
        public override void Dispose()
        {
            _timer.Elapsed -= _timer_Elapsed;
            _timer.Dispose();
            base.StateContainer.PropertyChanged -= this.StateContainer_PropertyChanged;
            base.Dispose();
        }
        protected override void OnInitialized()
        {
            _timer.Elapsed += _timer_Elapsed;
            base.StateContainer.PropertyChanged += this.StateContainer_PropertyChanged;
            base.OnInitialized();
        }

        private void StateContainer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(base.StateContainer.StorageProviderResponse))
            {
                this.UploadFileTask.Where(x => x.Status == Enums.UploadFileTaskStatusEnum.Pending)
                    .ToList()
                    .ForEach(x => x.Status = Enums.UploadFileTaskStatusEnum.Terminate);
                this.UploadFileTask.Clear();
                _table.ReloadServerData();
            }
        }

        private async void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _isUploading = true;
            _timer.Stop();

            var pending = this.UploadFileTask.Where(x => x.Status == Enums.UploadFileTaskStatusEnum.Pending).ToList();
            foreach (var task in pending)
            {
                if (task.Status == Enums.UploadFileTaskStatusEnum.Pending)
                {
                    task.Status = Enums.UploadFileTaskStatusEnum.Uploading;

                    if (task.File.ContentType.StartsWith("image"))
                    {
                        task.UploadFileConfig.Tags.Add("Require-Compress-Image");
                    }
                    var fileName = Path.Combine(Directory.GetCurrentDirectory(), Path.GetTempFileName());
                    try
                    {
                        await using FileStream fs = new FileStream(fileName, FileMode.Create);
                        await task.File.OpenReadStream(long.MaxValue).CopyToAsync(fs);
                        fs.Seek(0, SeekOrigin.Begin);
                        await this.HBKStorageApi.FileentitiesPOSTAsync(this.StateContainer.StorageProviderResponse.Storage_provider_id, task.File.Name, null, null, task.UploadFileConfig.Tags,
                            task.File.ContentType,
                            task.UploadFileConfig.CryptoMode,
                            new FileParameter(fs));
                        task.Status = Enums.UploadFileTaskStatusEnum.Complete;
                        task.CompleteDateTime = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        task.Exception = ex;
                        task.Status = Enums.UploadFileTaskStatusEnum.Terminate;
                    }
                    finally
                    {
                        File.Delete(fileName);
                    }
                }
                base.StateHasChanged();
            }

            if (this.UploadFileTask.Count(x => x.Status == Enums.UploadFileTaskStatusEnum.Pending) != 0)
            {
                _timer.Start();
            }
            else
            {
                _isUploading = false;
            }
        }
    }
}
