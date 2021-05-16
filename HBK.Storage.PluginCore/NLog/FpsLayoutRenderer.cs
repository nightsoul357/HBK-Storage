using Newtonsoft.Json.Linq;
using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.PluginCore.NLog
{
    [LayoutRenderer("plugin_ffpmeg_fps")]
    public class FpsLayoutRenderer : PluginLayoutRendererBase
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (base.TryFetchPluginLogEvent(logEvent, out PluginLogEvent pluginLogEvent))
            {
                if (pluginLogEvent.ExtendProperty == null)
                {
                    return;
                }

                JObject obj = ((JObject)pluginLogEvent.ExtendProperty);
                if (obj.ContainsKey("Fps"))
                {
                    builder.Append(obj["Fps"].Value<double?>());
                }
            }
        }
    }
}
