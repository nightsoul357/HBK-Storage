using HBK.Storage.Dashboard.Helpers;
using HBK.Storage.Dashboard.Models;
using HBK.Storage.Dashboard.Shared;

namespace HBK.Storage.Dashboard.Pages.Storages
{
    public partial class UpdateGoogleToken : HBKLayoutComponentBase
    {
        public string State { get; set; }
        public string Code { get; set; }
        public string Scope { get; set; }

        protected override void OnInitialized()
        {
            base.NavigationManager.TryGetQueryString("state", out string state);
            base.NavigationManager.TryGetQueryString("code", out string code);
            base.NavigationManager.TryGetQueryString("scope", out string scope);
            this.State = state;
            this.Code = code;
            this.Scope = scope;
            UpdateGoogleTokenResponse updateGoogleTokenResponse = new UpdateGoogleTokenResponse()
            {
                Code = code,
                Scope = scope,
                State = state
            };

            base.StateContainer.UpdateGoogleTokenResponse = updateGoogleTokenResponse;
#if !DEBUG
            base.NavigationService.CloseWindows();
#endif
        }
    }
}
