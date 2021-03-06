using HBK.Storage.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSeekPreviewPlugin.Models
{
    public class VideoSeekPreviewTaskManagerOptions : PluginTaskManagerOptions
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
        /// 取得或設定預覽圖的寬度
        /// </summary>
        public int PreviewWidth { get; set; }
        /// <summary>
        /// 取得或設定預覽圖的間隔
        /// </summary>
        public int PreviewInterval { get; set; } = 1;
        /// <summary>
        /// 取得或設定上傳任務數量
        /// </summary>
        public int UploadTaskCount { get; set; } = 5;
    }
}
