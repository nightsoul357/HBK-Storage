using HBK.Storage.VideoConvertM3U8Plugin.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoConvertM3U8Plugin.Models
{
    public class UploadFile
    {
        public string Filename { get; set; }
        public int Number { get; set; }
        public UploadFileTypeEnum Type { get; set; }
        public Guid M3U8FileEntityId { get; set; }
    }
}
