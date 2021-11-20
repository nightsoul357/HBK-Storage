using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.WebDAV
{
    /// <summary>
    /// WebDAV 檔案使用的流
    /// </summary>
    public class WebDAVFileStream : Stream
    {
        private long _position = 0;
        private readonly long _length;
        private readonly WebDAVSerivce _webDAVSerivce;
        private readonly string _key;
        private Stream _fileStream;

        /// <summary>
        /// 初始化 WebDAV 檔案使用的流
        /// <paramref name="webDAVSerivce"></paramref>
        /// <paramref name="key"></paramref>
        /// <paramref name="length"></paramref>
        /// </summary>
        public WebDAVFileStream(WebDAVSerivce webDAVSerivce, string key, long length)
        {
            _webDAVSerivce = webDAVSerivce;
            _key = key;
            _length = length;
        }
        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_fileStream == null)
            {
                _fileStream = _webDAVSerivce.DownloadPartialAsync(_key, _position, null).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            var read = _fileStream.Read(buffer, offset, count);
            _position += read;
            return read;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    break;
                case SeekOrigin.Current:
                    _position += offset;
                    break;
                case SeekOrigin.End:
                    _position = _length + offset;
                    break;
            }
            return _position;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }
            _webDAVSerivce.Dispose();
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        public override bool CanRead => true;
        /// <inheritdoc/>
        public override bool CanSeek => true;
        /// <inheritdoc/>
        public override bool CanWrite => false;
        /// <inheritdoc/>
        public override long Length => _length;
        /// <inheritdoc/>
        public override long Position { get => _position; set => _position = value; }
    }
}
