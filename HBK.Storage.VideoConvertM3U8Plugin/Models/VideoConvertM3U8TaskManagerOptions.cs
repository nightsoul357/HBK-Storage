using HBK.Storage.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoConvertM3U8Plugin.Models
{
    public class VideoConvertM3U8TaskManagerOptions : PluginTaskManagerOptions
    {
        public string FFmpegLocation { get; set; }
        public string WorkingDirectory { get; set; }
        public double TSInterval { get; set; }
    }
}
