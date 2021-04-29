using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoConvertM3U8Plugin.Models
{
    public class ConvertHandlerTaskModel
    {
        public string Identity { get; set; }
        public string VideoFileName { get; set; }
        public string OutputFileName { get; set; }
        public string FFmpegLocation { get; set; }
        public double TSInterval { get; set; }
        public string TempDirectory { get; set; }
    }
}
