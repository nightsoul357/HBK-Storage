using Newtonsoft.Json;
using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.PluginCore.NLog
{
    /// <summary>
    /// 插件 Log 自訂義基底型別
    /// </summary>
    public abstract class PluginLayoutRendererBase : LayoutRenderer
    {
        /// <summary>
        /// 嘗試從 <see cref="LogEventInfo"/> 取得 <see cref="PluginLogEvent"/>
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="pluginLogEvent"></param>
        /// <returns></returns>
        protected bool TryFetchPluginLogEvent(LogEventInfo logEvent, out PluginLogEvent pluginLogEvent)
        {
            pluginLogEvent = null;
            if (!logEvent.Properties.ContainsKey("EventId_Id"))
            {
                return false;
            }
            if (((int)logEvent.Properties["EventId_Id"]) != PluginLogEvent.PluginLogEventId)
            {
                return false;
            }
            pluginLogEvent = JsonConvert.DeserializeObject<PluginLogEvent>(logEvent.Properties["EventId_Name"].ToString());
            return true;
        }
    }
}
