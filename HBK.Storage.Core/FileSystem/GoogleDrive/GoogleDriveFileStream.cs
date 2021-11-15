using Google.Apis.Drive.v3;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.GoogleDrive
{
    /// <summary>
    /// Google Drive 檔案使用的流
    /// </summary>
    [Obsolete]
    public class GoogleDriveFileStream : Stream
    {
        private long _position;
        private long _length;
        private DriveService _driveService;
        private Google.Apis.Drive.v3.Data.File _file;
        private BlockingCollection<MemoryStream> _commonBuffer;
        private MemoryStream _currentBuffer;
        private Task _fetchTask;
        private CancellationTokenSource _fetchTaskCancellationTokenSource;
        private bool _isDisopsed = false;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="driveService"></param>
        /// <param name="file"></param>
        /// <param name="bufferCount"></param>
        /// <param name="bufferSize"></param>
        public GoogleDriveFileStream(DriveService driveService, Google.Apis.Drive.v3.Data.File file, int bufferSize = 1024 * 1024 * 10, int bufferCount = 2)
        {
            _file = file;
            _driveService = driveService;
            _length = file.Size.Value;
            _position = 0;
            _fetchTaskCancellationTokenSource = new CancellationTokenSource();
            _commonBuffer = new BlockingCollection<MemoryStream>(new ConcurrentQueue<MemoryStream>(), this.BufferCount);
            this.BufferSize = bufferSize;
            this.BufferCount = bufferCount;

        }

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_fetchTask == null)
            {
                _fetchTask = Task.Factory.StartNew(this.FetchDataTaskAsync, _fetchTaskCancellationTokenSource.Token);
            }

            if (_commonBuffer.IsAddingCompleted && _currentBuffer == null && _commonBuffer.Count == 0) // 特殊狀況，目標檔案為 0Bytes 時
            {
                return 0;
            }

            if (_commonBuffer.IsAddingCompleted &&
                _currentBuffer != null &&
                _currentBuffer.Position == _currentBuffer.Length &&
                _commonBuffer.Count == 0)
            {
                _currentBuffer.Close();
                _currentBuffer = null;
                return 0;
            }

            if (_commonBuffer.IsAddingCompleted && _commonBuffer.Count == 0 && _currentBuffer == null)
            {
                return 0;
            }

            if (_currentBuffer == null || _currentBuffer.Position == _currentBuffer.Length)
            {
                if (_currentBuffer != null)
                {
                    _currentBuffer.Close();
                }

                try
                {
                    _currentBuffer = _commonBuffer.Take();
                }
                catch (InvalidOperationException)
                {
                    return 0;
                }
                _currentBuffer.Seek(0, SeekOrigin.Begin);
            }

            int read = _currentBuffer.Read(buffer, offset, count);
            _position += read;
            return read;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_fetchTask != null)
            {
                throw new ApplicationException("開始讀取後禁止重新設定 Seek");
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

        private async Task FetchDataTaskAsync()
        {
            MemoryStream memoryStream = null;
            try
            {
                var request = _driveService.Files.Get(_file.Id);
                long ps = _position;
                while (ps < this.Length && !_fetchTaskCancellationTokenSource.IsCancellationRequested)
                {
                    memoryStream = new MemoryStream();
                    var read = await request.DownloadRangeAsync(memoryStream, new RangeHeaderValue(ps, ps + this.BufferSize - 1), _fetchTaskCancellationTokenSource.Token);
                    _commonBuffer.Add(memoryStream, _fetchTaskCancellationTokenSource.Token);
                    ps += read.BytesDownloaded;
                }

                _commonBuffer.CompleteAdding();
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is OperationCanceledException) // 中斷快取操作
            {
                if (memoryStream != null && memoryStream.Length != 0)
                {
                    memoryStream.Dispose();
                }
            }
        }

        /// <inheritdoc/>
        public override void Close()
        {
            _fetchTaskCancellationTokenSource.Cancel();
            base.Close();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!_isDisopsed)
            {
                _commonBuffer.Dispose();
                if (_currentBuffer != null)
                {
                    _currentBuffer.Dispose();
                }
                _driveService.Dispose();
                _file = null;
                _commonBuffer = null;
                _currentBuffer = null;
                _isDisopsed = true;
            }
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
