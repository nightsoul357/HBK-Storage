using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.AmazonS3
{
    /// <summary>
    /// AWS S3 檔案使用的流
    /// </summary>
    public class AWS3FileStream : Stream
    {
        private long _position = 0;
        private readonly long _length;
        private readonly AmazonS3Client _amazonS3Client;
        private readonly string _key;
        private readonly string _bucketName;
        private GetObjectResponse _getObjectResponse;
        /// <summary>
        /// 初始化 AWS S3 檔案使用的流
        /// </summary>
        /// <param name="amazonS3Client"></param>
        /// <param name="key"></param>
        /// <param name="bucketName"></param>
        /// <param name="size"></param>
        public AWS3FileStream(AmazonS3Client amazonS3Client, string key, string bucketName, long size)
        {
            _length = size;
            _amazonS3Client = amazonS3Client;
            _key = key;
            _bucketName = bucketName;
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_getObjectResponse == null)
            {
                _getObjectResponse = _amazonS3Client.GetObjectAsync(new GetObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = _key,
                    ByteRange = new ByteRange($"bytes={_position}-")
                }).Result;
            }

            var read = _getObjectResponse.ResponseStream.Read(buffer, offset, count);
            _position += read;
            return read;
        }
        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            _getObjectResponse = null;
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
