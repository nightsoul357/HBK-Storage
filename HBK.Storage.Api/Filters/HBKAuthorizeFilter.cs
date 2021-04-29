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
        private readonly AuthorizeKeyService _authorizeKeyService;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="authorizeKeyService"></param>
        public HBKAuthorizeFilter(AuthorizeKeyService authorizeKeyService)
        {
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
                context.Result = this.BuildForbiddenResult("APIKey Incorrect;");
                return;
            }
            if (authorizeKey.Status.HasFlag(AuthorizeKeyStatusEnum.Disable)) // 檢查是否已停用
            {
                context.Result = this.BuildForbiddenResult("APIKey was Disable;");
                return;
            }

            if (authorizeKey.Type == AuthorizeKeyTypeEnum.Root) // 若為 Root 權限，不檢查 Scope
            {
                return;
            }
            var operation = this.ConvertHTTPMethodTouthorizeKeyScopeOperationType(context.HttpContext.Request.Method);
            if (context.RouteData.Values.ContainsKey("storageProviderId"))
            {
                var storageProviderId = Guid.Parse(context.RouteData.Values["storageProviderId"].ToString());
                if (!(await _authorizeKeyService.IsExistAuthorizeKeyScopeByStorageProviderAsync(authorizeKey.AuthorizeKeyId, storageProviderId, operation)))
                {
                    context.Result = this.BuildForbiddenResult("Scope Incorrect;");
                    return;
                }
            }
            if (context.RouteData.Values.ContainsKey("storageGroupId"))
            {
                var storageGroupId = Guid.Parse(context.RouteData.Values["storageGroupId"].ToString());
                if (!(await _authorizeKeyService.IsExistAuthorizeKeyScopeByStorageGroupAsync(authorizeKey.AuthorizeKeyId, storageGroupId, operation)))
                {
                    context.Result = this.BuildForbiddenResult("Scope Incorrect;");
                    return;
                }
            }
            if (context.RouteData.Values.ContainsKey("storageId"))
            {
                var storageId = Guid.Parse(context.RouteData.Values["storageId"].ToString());
                if (!(await _authorizeKeyService.IsExistAuthorizeKeyScopeByStorageAsync(authorizeKey.AuthorizeKeyId, storageId, operation)))
                {
                    context.Result = this.BuildForbiddenResult("Scope Incorrect;");
                    return;
                }
            }
            if (context.RouteData.Values.ContainsKey("fileEntityId"))
            {
                var fileEntityId = Guid.Parse(context.RouteData.Values["fileEntityId"].ToString());
                if (!(await _authorizeKeyService.IsExistAuthorizeKeyScopeByFileEntityAsync(authorizeKey.AuthorizeKeyId, fileEntityId, operation)))
                {
                    context.Result = this.BuildForbiddenResult("Scope Incorrect;");
                    return;
                }
            }
            if (context.RouteData.Values.ContainsKey("fileAccessTokenId"))
            {
                var fileAccessTokenId = Guid.Parse(context.RouteData.Values["fileAccessTokenId"].ToString());
                if (!(await _authorizeKeyService.IsExistAuthorizeKeyScopeByFileAccessTokenAsync(authorizeKey.AuthorizeKeyId, fileAccessTokenId, operation)))
                {
                    context.Result = this.BuildForbiddenResult("Scope Incorrect;");
                    return;
                }
            }
        }
        private ContentResult BuildForbiddenResult(string message)
        {
            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.Forbidden,
                Content = message,
                ContentType = "text/plain"
            };
        }
        private AuthorizeKeyScopeOperationTypeEnum ConvertHTTPMethodTouthorizeKeyScopeOperationType(string httpMethod)
        {
            switch (httpMethod)
            {
                case "GET":
                    return AuthorizeKeyScopeOperationTypeEnum.Read;
                case "POST":
                    return AuthorizeKeyScopeOperationTypeEnum.Insert;
                case "PUT":
                    return AuthorizeKeyScopeOperationTypeEnum.Update;
                case "DELETE":
                    return AuthorizeKeyScopeOperationTypeEnum.Delete;
                default:
                    throw new NotImplementedException($"尚未實作 { httpMethod } 方法所對應的驗證方法");
            }
        }
    }
}