using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Drive.v3.FilesResource;

namespace HBK.Storage.Core.Helpers
{
    /// <summary>
    /// Google Drive 服務擴充功能
    /// </summary>
    public static class GoogleDriveServiceExtensions
    {
        /// <summary>
        /// 取得檔案流
        /// </summary>
        /// <param name="getRequest">檔案請求</param>
        /// <param name="range">下載範圍</param>
        /// <returns></returns>
        public static async Task<Stream> GetDownloadStreamAsync(this GetRequest getRequest, RangeItemHeaderValue range = null)
        {
            var url = getRequest.CreateRequest().RequestUri.AbsoluteUri;
            var uri = new UriBuilder(url);
            if (uri.Query == null || uri.Query.Length <= 1)
            {
                uri.Query = "alt=media";
            }
            else
            {
                // Remove the leading '?'. UriBuilder.Query doesn't round-trip.
                uri.Query = uri.Query.Substring(1) + "&alt=media";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, uri.Uri);
            if (range != null)
            {
                request.Headers.Range = new RangeHeaderValue();
                request.Headers.Range.Ranges.Add(range);
            }
            var response = await getRequest.Service.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
