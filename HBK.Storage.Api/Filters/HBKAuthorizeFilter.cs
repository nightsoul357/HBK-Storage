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
    public class HBKAuthorizeFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="configuration"></param>
        public HBKAuthorizeFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 執行驗證
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
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
            if (context.HttpContext.Request.Headers["HBKey"].First() != _configuration["HBKey"])
            {
                context.Result = new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    Content = "APIKey Incorrect;",
                    ContentType = "text/plain"
                };
                return;
            }
        }
    }
}
