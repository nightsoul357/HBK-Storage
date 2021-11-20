using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.WebDAV
{
    /// <summary>
    /// WebDAV 存取邏輯
    /// </summary>
    public class WebDAVSerivce : IDisposable
    {
        private readonly WebDAVOption _webDAVOption;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _nonTimoutHttpClient;
        /// <summary>
        /// 初始化 WebDAV 存取邏輯
        /// </summary>
        /// <param name="webDAVOption"></param>
        public WebDAVSerivce(WebDAVOption webDAVOption)
        {
            _webDAVOption = webDAVOption;
            _httpClient = _webDAVOption.HttpClientFunc();
            _nonTimoutHttpClient = _webDAVOption.HttpClientFunc();
            _nonTimoutHttpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        /// <summary>
        /// 取得檔案資訊
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public async Task<WebDAVFileInfo> GetFileInfoAsync(string subpath)
        {
            var request = this.BuildRequestMessage(subpath);
            request.Method = HttpMethod.Get;
            request.Headers.Add("depth", "0");
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            return new WebDAVFileInfo(this, subpath, response.Content.Headers.ContentLength.Value, response.Content.Headers.LastModified.Value, request.RequestUri.AbsoluteUri);
        }

        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="subpath"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public async Task<WebDAVFileInfo> PutAsync(string subpath, Stream fileStream)
        {
            var request = this.BuildRequestMessage(subpath);
            request.Method = HttpMethod.Put;
            request.Content = new StreamContent(fileStream);

            var response = await _nonTimoutHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            return await this.GetFileInfoAsync(subpath);
        }

        /// <summary>
        /// 分段上傳
        /// </summary>
        /// <param name="subpath"></param>
        /// <param name="fileStream"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task PutPartialAsync(string subpath, Stream fileStream, long from, long to)
        {
            var request = this.BuildRequestMessage(subpath);
            request.Method = HttpMethod.Put;
            request.Content = new StreamContent(fileStream);
            request.Content.Headers.ContentRange = new ContentRangeHeaderValue(from, to);
            request.Content.Headers.ContentLength = to - from;
            var response = await _nonTimoutHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// 檔案是否存在
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public async Task<bool> IsFileExistAsync(string subpath)
        {
            try
            {
                _ = await this.GetFileInfoAsync(subpath);
                return true;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public async Task DeleteAsync(string subpath)
        {
            var request = this.BuildRequestMessage(subpath);
            request.Method = HttpMethod.Delete;
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// 分段下載
        /// </summary>
        /// <param name="subpath"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadPartialAsync(string subpath, long? from, long? to)
        {
            var request = this.BuildRequestMessage(subpath);
            request.Headers.Range = new RangeHeaderValue();
            request.Headers.Range.Ranges.Add(new RangeItemHeaderValue(from, to));
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            return await response.Content.ReadAsStreamAsync();
        }
        private HttpRequestMessage BuildRequestMessage(string subpath)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.RequestUri = new Uri(_webDAVOption.Url + "/" + subpath);

            var basicToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_webDAVOption.Username}:{_webDAVOption.Password}"));
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", $"{basicToken}");
            return httpRequestMessage;
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            _httpClient.Dispose();
            _nonTimoutHttpClient.Dispose();
        }
    }
}
