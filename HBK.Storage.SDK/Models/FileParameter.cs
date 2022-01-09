using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models
{
    public class FileParameter
    {
        public FileParameter(Stream data)
            : this(data, null, null)
        {
        }

        public FileParameter(Stream data, string fileName)
            : this(data, fileName, null)
        {
        }

        public FileParameter(Stream data, string? fileName, string? contentType)
        {
            this.Data = data;
            this.FileName = fileName;
            this.ContentType = contentType;
        }

        public Stream Data { get; private set; }

        public string? FileName { get; private set; }

        public string? ContentType { get; private set; }
    }
}
