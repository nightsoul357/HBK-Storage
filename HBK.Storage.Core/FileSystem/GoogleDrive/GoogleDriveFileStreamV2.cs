using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using HBK.Storage.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.GoogleDrive
{
    /// <inheritdoc/>
    public class GoogleDriveFileStreamV2 : Stream
    {
        private long _position;
        private long _length;
        private DriveService _driveService;
        private Google.Apis.Drive.v3.Data.File _file;
        private Stream _fileStream;

        /// <inheritdoc/>
        public GoogleDriveFileStreamV2(DriveService driveService, Google.Apis.Drive.v3.Data.File file)
        {
            _file = file;
            _driveService = driveService;
            _length = file.Size.Value;
            _position = 0;
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
                var req = _driveService.Files.Get(_file.Id);
                _fileStream = req.GetDownloadStreamAsync(new RangeItemHeaderValue(_position, null)).ConfigureAwait(false).GetAwaiter().GetResult();
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
            _length = value;
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
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
