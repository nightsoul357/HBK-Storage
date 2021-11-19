using HBK.Storage.Dashboard.DataSource;
using MudBlazor;

namespace HBK.Storage.Dashboard.Pages.Authentication
{
    public partial class Login : HBKPageBase
    {
#if DEBUG
        public string Password { get; set; } = "ThisIsKeyForRoot_60990e6c-95f2-412e-8708-1961777e88fc";
#else
        public string Password { get; set; }
#endif
        public bool PasswordVisibility { get; set; }
        public InputType PasswordInput { get; set; } = InputType.Password;
        public string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;
        public bool IsRemeber { get; set; }

        /// <summary>
        /// 切換密碼顯示
        /// </summary>
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

        /// <summary>
        /// 登入
        /// </summary>
        public async Task SignInAsync()
        {
            try
            {
                var authorizeKeyResponse = await this.HBKStorageApi.KeyAsync(this.Password);
                authorizeKeyResponse.Key_value = this.Password;
                this.StateContainer.SetSignKey(authorizeKeyResponse, this.IsRemeber);
                base.NavigationManager.NavigateTo("/index");
            }
            catch (ApiException ex) when (ex.StatusCode == 404)
            {
                base.Snackbar.Add("API Key 不存在", Severity.Warning);
            }
        }
        protected override void OnParametersSet()
        {
            if (base.StateContainer.GetSignInKey() != null)
            {
                base.NavigationManager.NavigateTo("/index");
            }
        }
    }
}
