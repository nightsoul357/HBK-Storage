using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models
{
    public class FileResponse : IDisposable
    {
        private IDisposable _client;
        private IDisposable _response;

        public int StatusCode { get; private set; }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public Stream Stream { get; private set; }

        public bool IsPartial
        {
            get 
            { 
                return this.StatusCode == 206;
            }
        }

        public FileResponse(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, Stream stream, IDisposable client, IDisposable response)
        {
            this.StatusCode = statusCode;
            this.Headers = headers;
            this.Stream = stream;
            _client = client;
            _response = response;
        }

        public void Dispose()
        {
            this.Stream.Dispose();
            if (_response != null)
            {
                _response.Dispose();
            }
            if (_client != null)
            {
                _client.Dispose();
            }
        }
    }
}
