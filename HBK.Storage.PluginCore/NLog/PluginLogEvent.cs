using HBK.Storage.Adapter.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.PluginCore.NLog
{
    /// <summary>
    /// 插件事件資訊
    /// </summary>
    public class PluginLogEvent
    {
        /// <summary>
        /// 取得或設定插件識別碼
        /// </summary>
        public string Identity { get; set; }
        /// <summary>
        /// 取得或設定活動 ID
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 取得或設定檔案 ID
        /// </summary>
        public Guid FileEntityId { get; set; }
        /// <summary>
        /// 取得或設定檔案名稱
        /// </summary>
        public string FileEntityName { get; set; }
        /// <summary>
        /// 取得或設定額外資訊
        /// </summary>
        public object ExtendProperty { get; set; }
        /// <summary>
        /// 取得插件事件 ID
        /// </summary>
        public static int PluginLogEventId = 0x1001;
    }
}
