using HBK.Storage.Web.Authentications;
using HBK.Storage.Web.Containers;
using HBK.Storage.Web.DataSource;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Authentication
{
    public partial class Login
    {
        public string Password { get; set; } = "";
        public bool PasswordVisibility { get; set; }
        public bool IsRemeber { get; set; } = false;
        public InputType PasswordInput { get; set; } = InputType.Password;
        public string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        public ISnackbar Snackbar { get; set; }
        [Inject]
        public HBKStorageApi HBKStorageApi { get; set; }
        public HBKAuthStateProvider HBKAuthStateProvider
        {
            get
            {
                return (HBKAuthStateProvider)this.AuthenticationStateProvider;
            }
        }
        protected override Task OnInitializedAsync()
        {
            base.OnInitialized();
            if (this.HBKAuthStateProvider.GetLoginAuthorizeKey() != null)
            {
                this.NavigationManager.NavigateTo($"/");
            }
            return Task.CompletedTask;
        }

        public void TogglePasswordVisibility()
        {
            if (this.PasswordVisibility)
            {
                this.PasswordVisibility = false;
                this.PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                this.PasswordInput = InputType.Password;
            }
            else
            {
                this.PasswordVisibility = true;
                this.PasswordInputIcon = Icons.Material.Filled.Visibility;
                this.PasswordInput = InputType.Text;
            }
        }

        public async Task SignIn()
        {
            try
            {
                var authorizeKey = await this.HBKStorageApi.KeyAsync(this.Password);
                authorizeKey.Key_value = this.Password;
                this.HBKAuthStateProvider.SignIn(authorizeKey, this.IsRemeber);
                this.NavigationManager.NavigateTo("/");
            }
            catch (ApiException ex) when (ex.StatusCode == 404)
            {
                this.Snackbar.Add("API Key Incorrect", Severity.Error);
            }
        }
    }
}
