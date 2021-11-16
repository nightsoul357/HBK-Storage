using Blazored.LocalStorage;
using HBK.Storage.Web.Helpers;
using HBK.Storage.Web.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage
{
    public partial class UpdateGoogleToken
    {
        [Inject]
        public ISyncLocalStorageService SyncLocalStorageService { get; set; }
        public string State { get; set; }
        public string Code { get; set; }
        public string Scope { get; set; }
        [Inject]
        public Features.NavigationSerivce NavigationSerivce { get; set; }
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
            SyncLocalStorageService.SetItem(nameof(UpdateGoogleTokenResponse), updateGoogleTokenResponse);
            this.NavigationSerivce.CloseWindows();
        }
    }
}
