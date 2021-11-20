using HBK.Storage.Core.Enums;
using HBK.Storage.Core.FileSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Cryptography
{
    /// <inheritdoc/>
    public class CryptoStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly byte[] _iv;
        private readonly ICryptoTransform _cryptoTransform;
        private readonly ICryptoProvider _cryptoProvider;
        private long _currentCounter = 0;
        private readonly byte[] _counter = new byte[8];
        private readonly byte[] unreadBuffer = new byte[16];
        private int unreadBufferCount = 0;
        /// <inheritdoc/>
        public CryptoStream(Stream innerStream, ICryptoProvider cryptoProvider, byte[] key, byte[] iv)
        {
            _innerStream = innerStream;
            if (!(_innerStream is BufferStream))
            {
                _innerStream = new BufferStream(_innerStream, 65535);
            }

            _iv = iv;
            _cryptoProvider = cryptoProvider;

            _cryptoTransform = new CachedCryptoTransform(
                new Func<ICryptoTransform>(() => _cryptoProvider.GenerateEncryptTransform(key, new byte[16])),
                false);
        }
        /// <inheritdoc/>
        public override void Flush()
        {
            _innerStream.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int originalCount = count;
            int readCount = 0;

            if (unreadBufferCount != 0)
            {
                var size = Math.Min(unreadBufferCount, count);
                Array.Copy(unreadBuffer, 0, buffer, offset, size);
                unreadBufferCount -= size;
                count -= size;
                offset += size;
                readCount += size;
            }
            if (unreadBufferCount != 0)
            {
                return readCount;
            }

            // Validate count boundaries
            if (this.Length == 0 || (this.Position + count < this.Length) && count >= 16)
            {
                count -= (count % 16);  // Make count divisible by 16 for partial reads (as the minimal block is 16)
            }


            for (var pos = 0; pos < count; pos += 16)
            {
                var input = new byte[16];
                var output = new byte[input.Length];
                var inputLength = _innerStream.ReadAsync(input, 0, input.Length).ConfigureAwait(false).GetAwaiter().GetResult();
                if (inputLength != input.Length)
                {
                    inputLength += _innerStream.ReadAsync(input, inputLength, input.Length - inputLength).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                var ivCounter = new byte[16];
                Array.Copy(_iv, ivCounter, 8);
                Array.Copy(_counter, 0, ivCounter, 8, 8);

                var encryptedIvCounter = _cryptoTransform.TransformFinalBlock(ivCounter, 0, ivCounter.Length);
                for (int inputPos = 0; inputPos < inputLength; inputPos++)
                {
                    output[inputPos] = (byte)(encryptedIvCounter[inputPos] ^ input[inputPos]);
                }

                if (count < inputLength) // 還沒讀完
                {
                    Array.Copy(output, 0, buffer, (int)(offset + pos), count);
                    Array.Copy(output, count, unreadBuffer, 0, inputLength - count);
                    unreadBufferCount = inputLength - count;
                    readCount += count;
                }
                else
                {
                    Array.Copy(output, 0, buffer, (int)(offset + pos), inputLength);
                    readCount += inputLength;
                }

                this.IncrementCounter();
            }

            return readCount;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long times = offset / 16;
            _currentCounter = times;
            var counter = BitConverter.GetBytes(_currentCounter);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counter);
            }

            Array.Copy(counter, _counter, 8);
            _innerStream.Seek(offset - (offset % 16), origin);


            if (offset % 16 != 0)
            {
                var input = new byte[16];
                var output = new byte[input.Length];
                var inputLength = _innerStream.ReadAsync(input, 0, input.Length).ConfigureAwait(false).GetAwaiter().GetResult();
                if (inputLength != input.Length)
                {
                    inputLength += _innerStream.ReadAsync(input, inputLength, input.Length - inputLength).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                var ivCounter = new byte[16];
                Array.Copy(_iv, ivCounter, 8);
                Array.Copy(_counter, 0, ivCounter, 8, 8);
                var encryptedIvCounter = _cryptoTransform.TransformFinalBlock(ivCounter, 0, ivCounter.Length);
                for (int inputPos = 0; inputPos < inputLength; inputPos++)
                {
                    output[inputPos] = (byte)(encryptedIvCounter[inputPos] ^ input[inputPos]);
                }

                Array.Copy(output, (offset % 16), unreadBuffer, 0, output.Length - (offset % 16));
                unreadBufferCount = (int)(output.Length - (offset % 16));
                this.IncrementCounter();
            }

            return this.Position;
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

        private void IncrementCounter()
        {
            _currentCounter++;
            if ((_currentCounter & 0xFF) != 0xFF)
            {
                _counter[7]++;
            }
            else
            {
                var counter = BitConverter.GetBytes(_currentCounter);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(counter);
                }

                Array.Copy(counter, _counter, 8);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            _innerStream.Dispose();
            _cryptoTransform.Dispose();
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        public override void Close()
        {
            _innerStream.Close();
            base.Close();
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
