using HBK.Storage.Web.Containers;
using HBK.Storage.Web.DataSource;
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
        private readonly StateContainer _stateContainer;
        private readonly AuthenticationState _anonymous;
        public HBKAuthStateProvider(StateContainer stateContainer)
        {
            _stateContainer = stateContainer;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_stateContainer.AuthorizeKey == null)
            {
                return Task.FromResult(_anonymous);
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, _stateContainer.AuthorizeKey.Name),
            }, "Authorize Key");
            var user = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticationState(user));
        }
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="authorizeKeyResponse"></param>
        public void SignIn(AuthorizeKeyResponse authorizeKeyResponse)
        {
            _stateContainer.AuthorizeKey = authorizeKeyResponse;
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, authorizeKeyResponse.Name),
            }, "Authorize Key");
            var user = new ClaimsPrincipal(identity);

            base.NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        /// <summary>
        /// 登出
        /// </summary>
        public void SignOut()
        {
            _stateContainer.AuthorizeKey = null;
            base.NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }
        /// <summary>
        /// 取得已登入的驗證金鑰
        /// </summary>
        /// <returns></returns>
        public AuthorizeKeyResponse GetLoginAuthorizeKey()
        {
            return _stateContainer.AuthorizeKey;
        }
    }
}
