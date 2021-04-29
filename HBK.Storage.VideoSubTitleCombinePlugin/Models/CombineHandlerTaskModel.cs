using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSubTitleCombinePlugin.Models
{
    public class CombineHandlerTaskModel
    {
        public string Identity { get; set; }
        public string VideoFileName { get; set; }
        public string SubTitleFileName { get; set; }
        public string OutputFileName { get; set; }
        public string FFmpegLocation { get; set; }
        public string TempDirectory { get; set; }
    }
}
