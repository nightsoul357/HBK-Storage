using HBK.Storage.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoMetadataPlugin.Models
{
    public class VideoMetadataTaskManagerOptions : PluginTaskManagerOptions
    {
        /// <summary>
        /// 取得或設定工作目錄
        /// </summary>
        public string WorkingDirectory { get; set; }
        /// <summary>
        /// 取得或設定 FFmpeg 路徑
        /// </summary>
        public string FFmpegLocation { get; set; }
        /// <summary>
        /// 取得或設定產生預覽圖數量
        /// </summary>
        public int PreviewsCount { get; set; }
        /// <summary>
        /// 取得或設定例外檔案類型
        /// </summary>
        public List<string> ExceptionMimeTypes { get; set; }
    }
}
