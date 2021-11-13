using CG.Web.MegaApiClient;
using CG.Web.MegaApiClient.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Web.MegaApiClient
{
    public partial class MegaApiClient
    {
        public Task<Stream> DownloadStreamAsync(INode node, CancellationToken? cancellationToken = null)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node.Type != NodeType.File)
            {
                throw new ArgumentException("Invalid node");
            }

            if (!(node is INodeCrypto nodeCrypto))
            {
                throw new ArgumentException("node must implement INodeCrypto");
            }

            EnsureLoggedIn();

            var downloadRequest = new DownloadUrlRequest(node);
            var downloadResponse = Request<DownloadUrlResponse>(downloadRequest);

            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = new HttpMethod("Get");
            httpRequestMessage.RequestUri = new Uri(downloadResponse.Url);

            Stream dataStream = new BufferedStream(new HttpRangeStream(httpClient, httpRequestMessage));

            Stream resultStream = new MegaAesCtrStreamDecrypter(dataStream, downloadResponse.Size, nodeCrypto.Key, nodeCrypto.Iv, nodeCrypto.MetaMac);
            return Task.FromResult(resultStream);
        }
    }
}
