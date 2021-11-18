using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Models;
using HBK.Storage.Dashboard.Pages.StorageGroups;
using HBK.Storage.Dashboard.Pages.StorageProviders;
using HBK.Storage.Dashboard.Pages.Storages;
using HBK.Storage.Dashboard.Shared;
using MudBlazor;

namespace HBK.Storage.Dashboard.Services
{
    /// <summary>
    /// HBK Dialog 服務
    /// </summary>
    public class HBKDialogService
    {
        private readonly IDialogService _dialogService;
        public HBKDialogService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }
        public Task<DialogResult> ShowBasicAsync(string title, string content, string buttonText, Color buttonColor)
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", content);
            parameters.Add("ButtonText", buttonText);
            parameters.Add("Color", buttonColor);

            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.ExtraSmall,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<BasicDialog>(title, parameters, options);
            return dialog.Result;
        }
        public async Task<DialogParsingResult<StorageProviderResponse>> ShowAddStorageProviderDialogAsync()
        {
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Large,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<AddStorageProviderDialog>("", options);
            var dresult = await dialog.Result;
            var result = new DialogParsingResult<StorageProviderResponse>()
            {
                Cancelled = dresult.Cancelled
            };

            if (!dresult.Cancelled)
            {
                result.Data = (StorageProviderResponse)dresult.Data;
            }
            return result;
        }

        public async Task<DialogParsingResult<StorageGroupResponse>> ShowAddStorageGroupDialogAsync()
        {
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<AddEditStorageGroupDialog>("新增儲存群組", options);
            var dresult = await dialog.Result;
            var result = new DialogParsingResult<StorageGroupResponse>()
            {
                Cancelled = dresult.Cancelled
            };
            if (!dresult.Cancelled)
            {
                result.Data = (StorageGroupResponse)dresult.Data;
            }

            return result;
        }

        public async Task<DialogParsingResult<StorageGroupResponse>> ShowEditStorageGroupDialogAsync(StorageGroupResponse storageGroupResponse)
        {
            var parameters = new DialogParameters();
            parameters.Add("StorageGroupResponse", storageGroupResponse);
            parameters.Add("IsEditMode", true);
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<AddEditStorageGroupDialog>("編輯儲存群組", parameters, options);
            var dresult = await dialog.Result;
            var result = new DialogParsingResult<StorageGroupResponse>()
            {
                Cancelled = dresult.Cancelled
            };
            if (!dresult.Cancelled)
            {
                result.Data = (StorageGroupResponse)dresult.Data;
            }

            return result;
        }

        public async Task<DialogParsingResult<StorageResponse>> ShowAddStorageDialogAsync(Guid sotrageGroupId)
        {
            var parameters = new DialogParameters();
            parameters.Add("StorageGroupId", sotrageGroupId);
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<AddEditStorageDialog>("新增儲存個體", parameters, options);
            var dresult = await dialog.Result;
            var result = new DialogParsingResult<StorageResponse>()
            {
                Cancelled = dresult.Cancelled
            };
            if (!dresult.Cancelled)
            {
                result.Data = (StorageResponse)dresult.Data;
            }

            return result;
        }

        public async Task<DialogParsingResult<StorageResponse>> ShowEditStorageDialogAsync(Guid sotrageGroupId, StorageResponse storageResponse)
        {
            var parameters = new DialogParameters();
            parameters.Add("StorageGroupId", sotrageGroupId);
            parameters.Add("StorageResponse", storageResponse);
            parameters.Add("IsEditMode", true);
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<AddEditStorageDialog>("編輯儲存個體", parameters, options);
            var dresult = await dialog.Result;
            var result = new DialogParsingResult<StorageResponse>()
            {
                Cancelled = dresult.Cancelled
            };
            if (!dresult.Cancelled)
            {
                result.Data = (StorageResponse)dresult.Data;
            }

            return result;
        }
    }
}
