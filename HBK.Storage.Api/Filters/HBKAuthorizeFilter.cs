using HBK.Storage.Adapter.Enums;
using HBK.Storage.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Filters
{
    /// <summary>
    /// HBK 驗證器
    /// </summary>
    public class HBKAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly AuthorizeKeyService _authorizeKeyService;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="authorizeKeyService"></param>
        public HBKAuthorizeFilter(IConfiguration configuration, AuthorizeKeyService authorizeKeyService)
        {
            _configuration = configuration;
            _authorizeKeyService = authorizeKeyService;
        }
        /// <summary>
        /// 執行驗證
        /// </summary>
        /// <param name="context"></param>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            if (!context.HttpContext.Request.Headers.ContainsKey("HBKey"))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Content = "APIKey Missing;",
                    ContentType = "text/plain"
                };
                return;
            }

            var key = context.HttpContext.Request.Headers["HBKey"].First();
            var authorizeKey = await _authorizeKeyService.FindByKeyValueAsync(key);
            if (authorizeKey == null)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Content = "APIKey Incorrect;",
                    ContentType = "text/plain"
                };
                return;
            }
            if (authorizeKey.Status.HasFlag(AuthorizeKeyStatusEnum.Disable)) // 檢查是否已停用
            {
                context.Result = new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Content = "APIKey was disable;",
                    ContentType = "text/plain"
                };
                return;
            }

            if (authorizeKey.Type == AuthorizeKeyTypeEnum.Root) // 若為 Root 權限，不檢查 Scope
            {
                return;
            }
            // TODO :
            if (context.RouteData.Values.ContainsKey("storageProviderId"))
            {
                var storageProviderId = Guid.Parse(context.RouteData.Values["storageProviderId"].ToString());
                
            }
            if (context.RouteData.Values.ContainsKey("storageGroupId"))
            {
                var storageGroupId = Guid.Parse(context.RouteData.Values["storageGroupId"].ToString());
                
            }
            if (context.RouteData.Values.ContainsKey("storageId"))
            {
                var storageId = Guid.Parse(context.RouteData.Values["storageId"].ToString());
                
            }
            if (context.RouteData.Values.ContainsKey("fileEntityId"))
            {
                var fileEntityId = Guid.Parse(context.RouteData.Values["fileEntityId"].ToString());

            }
        }
    }
}