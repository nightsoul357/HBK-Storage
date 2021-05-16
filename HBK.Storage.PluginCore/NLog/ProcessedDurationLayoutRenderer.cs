﻿using Newtonsoft.Json.Linq;
using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.PluginCore.NLog
{
    [LayoutRenderer("plugin_ffpmeg_processed_duration")]
    public class ProcessedDurationLayoutRenderer : PluginLayoutRendererBase
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
                if (obj.ContainsKey("ProcessedDuration"))
                {
                    builder.Append(obj["ProcessedDuration"].Value<TimeSpan>().ToString("hh:mm:ss"));
                }
            }
        }
    }
}
