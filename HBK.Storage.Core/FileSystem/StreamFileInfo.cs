using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem
{
    /// <summary>
    /// 串流檔案資訊
    /// </summary>
    public class StreamFileInfo : AsyncFileInfo
    {
        private readonly Stream _stream;
        /// <summary>
        /// 初始化串流檔案資訊
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        public StreamFileInfo(Stream stream, long length)
        {
            _stream = stream;
            this.Length = length;
        }

        /// <inheritdoc/>
        public override Task<Stream> CreateReadStreamAsync()
        {
            return Task.FromResult(_stream);
        }
    }
}
