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
    [LayoutRenderer("plugin_ffpmeg_sizeKb")]
    public class SizeKbLayoutRenderer : PluginLayoutRendererBase
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
                if (obj.ContainsKey("SizeKb"))
                {
                    builder.Append(obj["SizeKb"].Value<int?>());
                }
            }
        }
    }
}
