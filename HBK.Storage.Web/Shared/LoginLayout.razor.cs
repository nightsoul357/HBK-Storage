using HBK.Notification.Web.Theme;
using MudBlazor;
using MudBlazor.ThemeManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Shared
{
    public partial class LoginLayout
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
    }
}
