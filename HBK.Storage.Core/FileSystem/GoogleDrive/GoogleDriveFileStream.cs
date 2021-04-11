using Google.Apis.Drive.v3;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.GoogleDrive
{
    /// <summary>
    /// Google Drive 檔案使用的流
    /// </summary>
    public class GoogleDriveFileStream : Stream
    {
        private long _position;
        private long _length;
        private DriveService _driveService;
        private Google.Apis.Drive.v3.Data.File _file;
        private BlockingCollection<MemoryStream> _commonBuffer;
        private MemoryStream _currentBuffer;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="driveService"></param>
        /// <param name="file"></param>
        /// <param name="bufferCount"></param>
        /// <param name="bufferSize"></param>
        public GoogleDriveFileStream(DriveService driveService, Google.Apis.Drive.v3.Data.File file, int bufferSize = 1024 * 1024 * 10, int bufferCount = 10)
        {
            _file = file;
            _driveService = driveService;
            _length = file.Size.Value;
            _position = 0;
            _commonBuffer = new BlockingCollection<MemoryStream>(new ConcurrentQueue<MemoryStream>(), this.BufferCount);
            this.BufferSize = bufferSize;
            this.BufferCount = bufferCount;
            Task.Factory.StartNew(this.FetchDataTask);
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_commonBuffer.IsAddingCompleted && _currentBuffer.Position == _currentBuffer.Length && _commonBuffer.Count == 0)
            {
                if (_currentBuffer != null)
                {
                    _currentBuffer.Close();
                    _currentBuffer = null;
                }
                return 0;
            }

            if (_currentBuffer == null || _currentBuffer.Position == _currentBuffer.Length)
            {
                if (_currentBuffer != null)
                {
                    _currentBuffer.Close();
                }

                _currentBuffer = _commonBuffer.Take();
                _currentBuffer.Seek(0, SeekOrigin.Begin);
            }

            int read = _currentBuffer.Read(buffer, offset, count);
            _position += read;
            return read;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
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

        private void FetchDataTask()
        {
            var request = _driveService.Files.Get(_file.Id);
            long ps = 0;
            while (ps != this.Length)
            {
                MemoryStream memoryStream = new MemoryStream();
                var read = request.DownloadRange(memoryStream, new RangeHeaderValue(ps, ps + this.BufferSize - 1));
                _commonBuffer.Add(memoryStream);
                ps += read.BytesDownloaded;
            }
            _commonBuffer.CompleteAdding();
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Length => _length;

        /// <inheritdoc/>
        public override long Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// 取得緩衝區大小
        /// </summary>
        public int BufferSize { get; private set; } = 1024 * 1024 * 10;
        /// <summary>
        /// 取得緩衝區數量
        /// </summary>
        public int BufferCount { get; private set; } = 10;
    }
}
