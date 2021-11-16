using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CG.Web.MegaApiClient
{
    /// <summary>
    /// 使用 HTTP Range 標頭分段讀取 <see cref="HttpRequestMessage"/> 的流 
    /// </summary>
    internal class HttpRangeStream : Stream
    {
        private long _position = 0;
        private readonly HttpClient _httpClient;
        private readonly HttpRequestMessage _httpRequestMessage;
        private readonly HttpCompletionOption _httpCompletionOption;
        private bool disposed = false;
        private Stream _innerStream;
        /// <summary>
        /// 初始化使用 HTTP Range 標頭分段讀取 <see cref="HttpRequestMessage"/> 的流 
        /// </summary>
        /// <param name="httpClient">用於發送請求的 <seealso cref="HttpClient"/></param>
        /// <param name="httpRequestMessage">請求訊息內容</param>
        /// <param name="httpCompletionOption"></param>
        public HttpRangeStream(HttpClient httpClient, HttpRequestMessage httpRequestMessage, HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead)
        {
            _httpClient = httpClient;
            _httpRequestMessage = httpRequestMessage;
            _httpCompletionOption = httpCompletionOption;
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_innerStream == null)
            {
                _httpRequestMessage.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue();
                _httpRequestMessage.Headers.Range.Ranges.Add(new System.Net.Http.Headers.RangeItemHeaderValue(_position, null));
                var responseMessage = _httpClient.SendAsync(_httpRequestMessage, _httpCompletionOption).ConfigureAwait(false).GetAwaiter().GetResult();
                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(responseMessage.ReasonPhrase);
                }
                _innerStream = responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            int read = _innerStream.Read(buffer, offset, count);
            _position += read;
            return read;
        }
        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_innerStream != null)
            {
                _innerStream.Close();
                _innerStream.Dispose();
                _innerStream = null;
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
                    _position = this.Length + offset;
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
        public override void Flush()
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
        public override long Length => 0;
        /// <inheritdoc/>
        public override long Position { get => _position; set => _position = value; }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (_innerStream != null)
                {
                    _innerStream.Dispose();
                }
                _httpClient.Dispose();
                _httpRequestMessage.Dispose();
                disposed = true;
            }
            base.Dispose(disposing);
        }

        public override void Close()
        {
            if (_innerStream != null)
            {
                _innerStream.Close();
            }
            base.Close();
        }
    }
}