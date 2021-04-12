using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Middlewares
{
    /// <summary>
    /// 處理全域例外的中介層
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        public GlobalExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<GlobalExceptionMiddleware>();
        }

        /// <summary>
        /// 執行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "攔截到未預期的錯誤");
                throw; // TODO : 統一回應格式
            }
        }
    }
}
