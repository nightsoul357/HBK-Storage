using HBK.Notification.Web.Theme;
using HBK.Storage.Web.Authentications;
using HBK.Storage.Web.DataSource;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.ThemeManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Shared
{
    public partial class MainLayout
    {
        private ThemeManagerTheme _themeManager = new ThemeManagerTheme
        {
            Theme = new MudBlazorAdminDashboard(),
            DrawerClipMode = DrawerClipMode.Always,
            FontFamily = "Montserrat",
            DefaultBorderRadius = 6,
            AppBarElevation = 1,
            DrawerElevation = 1
        };

        private bool _drawerOpen = true;
        private bool _themeManagerOpen = false;
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public HBKAuthStateProvider HBKAuthStateProvider
        {
            get
            {
                return (HBKAuthStateProvider)this.AuthenticationStateProvider;
            }
        }
        public AuthorizeKeyResponse AuthorizeKey
        {
            get
            {
                return this.HBKAuthStateProvider.GetLoginAuthorizeKey();
            }
        }

        public void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        public void OpenThemeManager(bool value)
        {
            _themeManagerOpen = value;
        }

        public void UpdateTheme(ThemeManagerTheme value)
        {
            _themeManager = value;
            base.StateHasChanged();
        }
        public void SignOut()
        {
            ((HBKAuthStateProvider)this.AuthenticationStateProvider).SignOut();
            this.NavigationManager.NavigateTo($"/authentication/login");
        }
        protected override void OnInitialized()
        {
            base.StateHasChanged();
        }
        protected override Task OnInitializedAsync()
        {
            base.OnInitialized();
            if (this.AuthorizeKey == null)
            {
                this.NavigationManager.NavigateTo($"authentication/login");
            }
            return Task.CompletedTask;
        }
    }
}
