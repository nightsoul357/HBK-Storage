using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Authentications
{
    /// <summary>
    /// HBK 驗證狀態提供者
    /// </summary>
    public class HBKAuthStateProvider : AuthenticationStateProvider
    {
        /// <summary>
        /// 取得或設定 API Key
        /// </summary>
        public string ApiKey { get; set; }
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (String.IsNullOrEmpty(this.ApiKey))
            {
                var anonymous = new ClaimsIdentity();
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "HBK Storage"),
            }, "Fake authentication type");

            var user = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticationState(user));
        }
    }
}
