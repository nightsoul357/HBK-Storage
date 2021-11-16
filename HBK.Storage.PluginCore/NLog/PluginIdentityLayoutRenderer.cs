﻿using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.PluginCore.NLog
{
    /// <summary>
    /// 插件識別碼渲染
    /// </summary>
    [LayoutRenderer("plugin_identity")]
    public class PluginIdentityLayoutRenderer : PluginLayoutRendererBase
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (base.TryFetchPluginLogEvent(logEvent, out PluginLogEvent pluginLogEvent))
            {
                builder.Append(pluginLogEvent.Identity);
            }
        }
    }
}
