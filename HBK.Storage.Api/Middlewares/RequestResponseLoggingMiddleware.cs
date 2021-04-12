using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Middlewares
{
    /// <summary>
    /// 紀錄請求和回應內容的中介層
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
        }

        /// <summary>
        /// 執行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("收到請求");
            await _next(context);
            _logger.LogInformation("送出回應");
        }
    }
}
