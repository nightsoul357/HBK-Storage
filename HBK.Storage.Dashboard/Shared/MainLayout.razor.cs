using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Pages;
using HBK.Storage.Dashboard.Themes;

namespace HBK.Storage.Dashboard.Shared
{
    public partial class MainLayout : HBKLayoutComponentBase
    {
        private HBKStorageDashboardTheme _theme = new();
        /// <summary>
        /// 取得已登入的金鑰
        /// </summary>
        public AuthorizeKeyResponse? SignInKey { get; set; }
        private bool _drawerOpen = true;

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        protected override void OnInitialized()
        {
            base.StateHasChanged();
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void SignOut()
        {
            base.StateContainer.SetSignKey(null, true);
            base.NavigationManager.NavigateTo("/");
        }
        protected override void OnParametersSet()
        {
            this.SignInKey = this.StateContainer.GetSignInKey();
            base.OnParametersSet();
        }
    }
}
