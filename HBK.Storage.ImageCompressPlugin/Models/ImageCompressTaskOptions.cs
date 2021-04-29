using HBK.Storage.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.ImageCompressPlugin.Models
{
    public class ImageCompressTaskOptions : PluginTaskManagerOptions
    {
        public List<CompressLevelModel> CompressLevels { get; set; }
    }
}
