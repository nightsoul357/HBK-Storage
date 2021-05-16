using HBK.Storage.Adapter.Storages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.PluginCore.NLog
{
    /// <summary>
    /// Log 事件
    /// </summary>
    public static class LogEvent
    {
        /// <summary>
        /// 產生差件的 Log 事件
        /// </summary>
        /// <param name="identity">差件識別代碼</param>
        /// <param name="activityId">活動 ID</param>
        /// <param name="fileEntity">檔案識別代碼</param>
        /// <param name="extendProperty">額外資訊</param>
        /// <returns></returns>
        public static EventId BuildPluginLogEvent(string identity, Guid activityId, FileEntity fileEntity, object extendProperty)
        {
            return new EventId(PluginLogEvent.PluginLogEventId, JsonConvert.SerializeObject(new PluginLogEvent
            {
                Identity = identity,
                ActivityId = activityId,
                FileEntityId = fileEntity.FileEntityId,
                FileEntityName = fileEntity.Name,
                ExtendProperty = extendProperty
            }));
        }
    }
}
