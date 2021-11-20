using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem
{
    /// <summary>
    /// 具有 Memory Buffer 的流
    /// </summary>
    public class BufferStream : Stream
    {
        private readonly int _bufferSize;
        private readonly Stream _innerStream;
        private byte[] _currentBuffer;
        private int _streamBufferDataStartIndex;
        private int _streamBufferDataCount;

        /// <summary>
        /// 初始化具有 Memory Buffer 的流
        /// </summary>
        /// <param name="innerStream"></param>
        /// <param name="bufferSize"></param>
        public BufferStream(Stream innerStream, int bufferSize = 1024 * 1024 * 8)
        {
            _innerStream = innerStream;
            _bufferSize = bufferSize;
            _currentBuffer = new byte[_bufferSize];
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            _innerStream.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var totalReadCount = 0;

            while (true)
            {
                var copyCount = Math.Min(_streamBufferDataCount, count);
                if (copyCount != 0)
                {
                    Array.Copy(_currentBuffer, _streamBufferDataStartIndex, buffer, offset, copyCount);
                    offset += copyCount;
                    count -= copyCount;
                    _streamBufferDataStartIndex += copyCount;
                    _streamBufferDataCount -= copyCount;
                    totalReadCount += copyCount;
                }

                if (count == 0)
                {
                    break; // Request has been filled.
                }

                this.ReadToBufferAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                if (_streamBufferDataCount == 0)
                {
                    break; // End of stream.
                }
            }

            return totalReadCount;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }

        private async Task ReadToBufferAsync()
        {
            _streamBufferDataStartIndex = 0;
            _streamBufferDataCount = 0;

            while (true)
            {
                var startOfFreeSpace = _streamBufferDataStartIndex + _streamBufferDataCount;

                var availableSpaceInBuffer = _currentBuffer.Length - startOfFreeSpace;
                if (availableSpaceInBuffer == 0)
                {
                    break; // Buffer is full.
                }

                var readCount = await _innerStream.ReadAsync(_currentBuffer, startOfFreeSpace, availableSpaceInBuffer);
                if (readCount == 0)
                {
                    break; // End of stream.
                }

                _streamBufferDataCount += readCount;
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            _innerStream.Dispose();
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        public override bool CanRead => _innerStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => _innerStream.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => _innerStream.CanWrite;
        /// <inheritdoc/>
        public override long Length => _innerStream.Length;
        /// <inheritdoc/>
        public override long Position { get => _innerStream.Position; set => _innerStream.Position = value; }
    }
}
