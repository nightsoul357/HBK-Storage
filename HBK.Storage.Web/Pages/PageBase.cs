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

namespace HBK.Storage.Web.Pages
{
    /// <summary>
    /// 頁面的基底型別
    /// </summary>
    public abstract class PageBase<T> : ComponentBase
    {
        [Inject]
        protected HBKStorageApi HBKStorageApi { get; set; }
        [Inject]
        protected HBKDialogService HBKDialogService { get; set; }
        [Inject]
        protected StateContainer StateContainer { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected ISnackbar Snackbar { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var stateValidation = Attribute.GetCustomAttribute(typeof(T), typeof(StateValidationAttribute));
            if (stateValidation != null)
            {
                this.ValidState((StateValidationAttribute)stateValidation);
            }
        }

        private void ValidState(StateValidationAttribute stateValidation)
        {
            if (stateValidation.IsStorageProviderValid && this.StateContainer.StorageProvider == null)
            {
                this.NavigationManager.NavigateTo("storageProvider/manage");
                return;
            }
            if (stateValidation.IsStorageGroupValid && this.StateContainer.StorageGroup == null)
            {
                this.NavigationManager.NavigateTo("storageGroup/manage");
                return;
            }
            if (stateValidation.AuthorizeKeyType != null && this.StateContainer.AuthorizeKey.Type != stateValidation.AuthorizeKeyType)
            {
                this.NavigationManager.NavigateTo("/");
                return;
            }
        }
    }
}
