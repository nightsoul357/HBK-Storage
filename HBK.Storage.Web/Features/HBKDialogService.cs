﻿using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.Pages.AuthorizeKey;
using HBK.Storage.Web.Pages.FileEntity;
using HBK.Storage.Web.Pages.Storage;
using HBK.Storage.Web.Pages.StorageGroup;
using HBK.Storage.Web.Pages.StorageProvider;
using HBK.Storage.Web.Shared.Component;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Features
{
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

        public Task<DialogResult> ShowEditStorageProviderAsync(StorageProviderResponse storageProvider)
        {
            var parameters = new DialogParameters();
            parameters.Add("EditStorageProvider", storageProvider);

            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<EditStorageProviderDialog>("編輯儲存提供者", parameters, options);
            return dialog.Result;
        }

        public Task<DialogResult> ShowAddStorageProviderAsync()
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
            var dialog = _dialogService.Show<EditStorageProviderDialog>("新增儲存提供者", options);
            return dialog.Result;
        }

        public Task<DialogResult> ShowEditStorageGroupAsync(StorageGroupExtendPropertyResponse storageGroup)
        {
            var parameters = new DialogParameters();
            parameters.Add("EditStorageGroup", storageGroup);
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<EditStorageGroupDialog>("編輯儲存群組", parameters, options);
            return dialog.Result;
        }
        public Task<DialogResult> ShowAddStorageGroupAsync()
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
            var dialog = _dialogService.Show<EditStorageGroupDialog>("新增儲存群組", options);
            return dialog.Result;
        }

        public Task<DialogResult> ShowEditStorageAsync(StorageExtendPropertyResponse storage)
        {
            var parameters = new DialogParameters();
            parameters.Add("EditStorage", storage);
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<EditStorageDialog>("編輯儲存個體", parameters, options);
            return dialog.Result;
        }

        public Task<DialogResult> ShowAddStorageAsync()
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
            var dialog = _dialogService.Show<EditStorageDialog>("新增儲存個體", options);
            return dialog.Result;
        }

        public Task<DialogResult> ShowFileEntityInformationAsync(FileEntityResponse fileEntity)
        {
            var parameters = new DialogParameters();
            parameters.Add("FileEntity", fileEntity);
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.ExtraLarge,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<FileEntityInforamtionDialog>("檔案資訊", parameters, options);
            return dialog.Result;
        }

        public Task<DialogResult> ShowPublishAccessTokenAsync(FileEntityResponse fileEntity)
        {
            var parameters = new DialogParameters();
            parameters.Add("FileEntity", fileEntity);
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<PublishAccessTokenDialog>("發行檔案連結", parameters, options);
            return dialog.Result;
        }

        public Task<DialogResult> ShowEditAuthorizeKeyAsync(AuthorizeKeyResponse authorizeKey)
        {
            var parameters = new DialogParameters();
            parameters.Add("EditAuthorizeKey", authorizeKey);
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                CloseButton = true,
                DisableBackdropClick = true,
                NoHeader = false,
                Position = DialogPosition.Center
            };
            var dialog = _dialogService.Show<EditAuthorizeKeyDialog>("驗證金鑰內容", parameters, options);
            return dialog.Result;
        }

        public Task<DialogResult> ShowAddAuthorizeKeyAsync()
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
            var dialog = _dialogService.Show<EditAuthorizeKeyDialog>("編輯驗證金鑰", options);
            return dialog.Result;
        }
    }
}