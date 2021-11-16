using Blazored.LocalStorage;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.DataSource.Models;
using HBK.Storage.Web.Features;
using HBK.Storage.Web.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage.Credential
{
    public partial class GoogleDriveCredentailComponent : ICredentialsComponent, IDisposable
    {
        /// <summary>
        /// 取得或設定父資料夾 ID
        /// </summary>
        public string Parent { get; set; }
        /// <summary>
        /// 取得或設定存取金鑰
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 取得或設定存取密鑰
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 取得或設定 Token 資訊
        /// </summary>
        public Dictionary<string, string> Tokens { get; set; }
        public string RedirectUri { get; set; }

        public IAuthorizationCodeFlow Flow { get; set; }
        [Inject]
        public ISyncLocalStorageService SyncLocalStorageService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public NavigationSerivce NavigationSerivce { get; set; }
        public System.Timers.Timer Timer { get; set; }
        public string TokenMessage { get; set; }
        [Inject]
        protected ISnackbar Snackbar { get; set; }
        private bool isFirstRender = true;
        [Parameter]
        public StorageCredentialsBase Credential
        {
            get
            {
                return new GoogleDriveCredentials()
                {
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    Parent = this.Parent,
                    Storage_type = StorageType.Google_drive,
                    Tokens = this.Tokens,
                    User = this.User
                };
            }
            set
            {
                if (value != null && isFirstRender)
                {
                    this.ClientId = ((GoogleDriveCredentials)value).ClientId;
                    this.ClientSecret = ((GoogleDriveCredentials)value).ClientSecret;
                    this.Parent = ((GoogleDriveCredentials)value).Parent;
                    this.Tokens = ((GoogleDriveCredentials)value).Tokens;
                    this.User = ((GoogleDriveCredentials)value).User;
                    isFirstRender = false;
                }
            }
        }
        protected override void OnInitialized()
        {
            this.RedirectUri = this.NavigationManager.BaseUri + "signin-google";
            this.Timer = new System.Timers.Timer(1000);
            this.Timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _ = base.InvokeAsync(async () =>
            {
                var result = this.SyncLocalStorageService.GetItem<UpdateGoogleTokenResponse>(nameof(UpdateGoogleTokenResponse));
                if (result != null)
                {
                    this.Timer.Stop();
                    try
                    {
                        var token = await this.Flow.ExchangeCodeForTokenAsync(this.User, result.Code, this.RedirectUri, System.Threading.CancellationToken.None);
                        this.Tokens = new Dictionary<string, string>()
                        {
                            [this.User] = JsonConvert.SerializeObject(token)
                        };
                        this.TokenMessage = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        this.TokenMessage = $"更新失敗 {ex.Message}";
                    }
                    finally
                    {
                        this.StateHasChanged();
                    }
                    this.SyncLocalStorageService.RemoveItem(nameof(UpdateGoogleTokenResponse));
                }
            });

        }
        public void GetToken()
        {
            if (string.IsNullOrEmpty(this.ClientId) || string.IsNullOrEmpty(this.ClientSecret) || string.IsNullOrEmpty(this.User))
            {
                this.Snackbar.Add("請先填寫 Client ID、Client Secret 以及 User", Severity.Warning);
                return;
            }
            this.Flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret
                },
                Scopes = new[] { DriveService.Scope.Drive },
                DataStore = new NullDataStore(),
            });

            AuthorizationCodeWebApp authorizationCodeWebApp = new AuthorizationCodeWebApp(this.Flow, this.RedirectUri, string.Empty);
            var result = authorizationCodeWebApp.AuthorizeAsync(this.User, System.Threading.CancellationToken.None).Result;
            if (result.Credential != null)
            {
                this.Tokens = new Dictionary<string, string>()
                {
                    [this.User] = JsonConvert.SerializeObject(result.Credential.Token)
                };
                base.StateHasChanged();
            }
            else
            {
                this.Timer.Start();
                this.NavigationSerivce.NavigateToAsync(result.RedirectUri);
            }
        }

        public void Dispose()
        {
            this.Timer.Dispose();
        }
    }
}
