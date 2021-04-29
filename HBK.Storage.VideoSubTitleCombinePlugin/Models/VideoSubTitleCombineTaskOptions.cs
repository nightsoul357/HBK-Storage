using HBK.Storage.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSubTitleCombinePlugin.Models
{
    public class VideoSubTitleCombineTaskOptions : PluginTaskManagerOptions
    {
        /// <summary>
        /// 取得或設定工作目錄
        /// </summary>
        public string WorkingDirectory { get; set; }
        /// <summary>
        /// 取得或設定 FFmpeg 檔案位置
        /// </summary>
        public string FFmpegLocation { get; set; }
    }
}
