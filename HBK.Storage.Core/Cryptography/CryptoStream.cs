using HBK.Storage.Core.Enums;
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
        /// <inheritdoc/>
        public CryptoStream(Stream innerStream, ICryptoProvider cryptoProvider, byte[] key, byte[] iv)
        {
            _innerStream = innerStream;
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
            // Validate count boundaries
            if (this.Length == 0 || (this.Position + count < this.Length))
            {
                count -= (count % 16);  // Make count divisible by 16 for partial reads (as the minimal block is 16)
            }

            int readCount = 0;

            for (var pos = 0; pos < count; pos += 16)
            {
                var input = new byte[16];
                var output = new byte[input.Length];
                var inputLength = _innerStream.ReadAsync(input, 0, input.Length).ConfigureAwait(false).GetAwaiter().GetResult();
                if (inputLength != input.Length)
                {
                    inputLength += _innerStream.ReadAsync(input, inputLength, input.Length - inputLength).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                readCount += inputLength;
                var ivCounter = new byte[16];
                Array.Copy(_iv, ivCounter, 8);
                Array.Copy(_counter, 0, ivCounter, 8, 8);

                var encryptedIvCounter = _cryptoTransform.TransformFinalBlock(ivCounter, 0, ivCounter.Length);
                for (int inputPos = 0; inputPos < inputLength; inputPos++)
                {
                    output[inputPos] = (byte)(encryptedIvCounter[inputPos] ^ input[inputPos]);
                }

                Array.Copy(output, 0, buffer, (int)(offset + pos), inputLength);
                this.IncrementCounter();
            }

            return readCount;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset % 16 != 0)
            {
                throw new ArgumentException(nameof(offset));
            }

            long times = offset / 16;
            _currentCounter = times;
            var counter = BitConverter.GetBytes(_currentCounter);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counter);
            }

            Array.Copy(counter, _counter, 8);

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
