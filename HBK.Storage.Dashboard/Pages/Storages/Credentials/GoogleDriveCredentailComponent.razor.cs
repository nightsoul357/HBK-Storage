using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.DataSource.Models;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;

namespace HBK.Storage.Dashboard.Pages.Storages.Credentials
{
    public partial class GoogleDriveCredentailComponent : CredentialComponentBase<GoogleDriveCredentials>
    {
        public string TokenUpdateMessage { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public System.Timers.Timer Timer { get; set; }
        public IAuthorizationCodeFlow Flow { get; set; }

        public async Task UpdateTokenAsync()
        {
            if (string.IsNullOrEmpty(base._credentials.ClientId) || string.IsNullOrEmpty(base._credentials.ClientSecret) || string.IsNullOrEmpty(base._credentials.User))
            {
                this.Snackbar.Add("請先填寫 Client ID、Client Secret 以及 User", Severity.Warning);
                return;
            }

            this.Flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = base._credentials.ClientId,
                    ClientSecret = base._credentials.ClientSecret
                },
                Scopes = new[] { DriveService.Scope.Drive },
                DataStore = new NullDataStore(),
            });

            AuthorizationCodeWebApp authorizationCodeWebApp = new AuthorizationCodeWebApp(this.Flow, this.RedirectUri, string.Empty);
            var result = await authorizationCodeWebApp.AuthorizeAsync(base._credentials.User, System.Threading.CancellationToken.None);
            if (result.Credential != null)
            {
                base._credentials.Tokens = new Dictionary<string, string>()
                {
                    [base._credentials.User] = JsonConvert.SerializeObject(result.Credential.Token)
                };
                base.StateHasChanged();
            }
            else
            {
                this.Timer.Start();
                base.NavigationService.NavigateToAsync(result.RedirectUri); // don't wait javecript return
            }

        }
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _ = base.InvokeAsync(async () =>
            {
                var result = base.StateContainer.UpdateGoogleTokenResponse;
                if (result != null)
                {
                    this.Timer.Stop();
                    try
                    {
                        var token = await this.Flow.ExchangeCodeForTokenAsync(base._credentials.User, result.Code, this.RedirectUri, System.Threading.CancellationToken.None);
                        base._credentials.Tokens = new Dictionary<string, string>()
                        {
                            [base._credentials.User] = JsonConvert.SerializeObject(token)
                        };
                        this.TokenUpdateMessage = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        this.TokenUpdateMessage = $"更新失敗 {ex.Message}";
                    }
                    finally
                    {
                        this.StateHasChanged();
                    }

                    base.StateContainer.UpdateGoogleTokenResponse = null;
                }
            });

        }

        public override void Dispose()
        {
            this.Timer.Dispose();
            base.Dispose();
        }

        protected override void OnInitialized()
        {
            this.RedirectUri = base.NavigationManager.BaseUri + "signin-google";
            this.Timer = new System.Timers.Timer(1000);
            this.Timer.Elapsed += this.Timer_Elapsed;
            base.OnInitialized();
        }
    }
}
