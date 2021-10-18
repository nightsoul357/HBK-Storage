using HBK.Notification.Web.Theme;
using MudBlazor;
using MudBlazor.ThemeManager;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool _drawerOpen = true;
        public bool _themeManagerOpen = false;

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        void OpenThemeManager(bool value)
        {
            _themeManagerOpen = value;
        }

        void UpdateTheme(ThemeManagerTheme value)
        {
            _themeManager = value;
            base.StateHasChanged();
        }

        protected override void OnInitialized()
        {
            base.StateHasChanged();
        }
    }
}
