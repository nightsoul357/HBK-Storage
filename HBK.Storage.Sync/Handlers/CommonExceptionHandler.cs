using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Sync.Handlers
{
    /// <summary>
    /// 一般化的例外處理器
    /// </summary>
    public class CommonExceptionHandler
    {
        private readonly ILogger<CommonExceptionHandler> _logger;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="logger"></param>
        public CommonExceptionHandler(ILogger<CommonExceptionHandler> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 處理例外
        /// </summary>
        /// <param name="ex"></param>
        public void Handle(Exception ex)
        {
            _logger.LogError(ex, "發生未預期的例外");
        }
    }
}
