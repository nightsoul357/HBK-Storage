using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Sync.Extensions
{
    /// <summary>
    /// Logger 的輔助紀錄類別
    /// </summary>
    public static class LoggerExtensions
    {
        public static void LogCustomInformation(this ILogger logger, string identity, string taskName, Guid taskIdentity, string message, params object[] args)
        {
            logger.LogInformation($"[{identity}][{taskName}][{taskIdentity.ToString()}]{message}", args);
        }

        public static void LogCustomWarning(this ILogger logger, string identity, string taskName, Guid taskIdentity, string message, params object[] args)
        {
            logger.LogWarning($"[{identity}][{taskName}][{taskIdentity.ToString()}]{message}", args);
        }
        public static void LogCustomError(this ILogger logger, string identity, string taskName, Guid taskIdentity, Exception ex, string message, params object[] args)
        {
            logger.LogError(ex, $"[{identity}][{taskName}][{taskIdentity.ToString()}]{message}", args);
        }
    }
}
