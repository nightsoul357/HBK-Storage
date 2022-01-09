using HBK.Storage.SDK.Models.FileEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Services
{
    /// <summary>
    /// 檔案權杖服務
    /// </summary>
    public class FileAccessTokenService : ServiceBase
    {
        public FileAccessTokenService(string baseUrl, Func<HttpClient> httpClientFunc, string apiKey)
            : base(baseUrl, httpClientFunc, apiKey)
        {
        }

        /// <summary>
        /// 取得指定 ID 之檔案存取權杖
        /// </summary>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<FileAccessTokenResponse> FindByIdAsync(System.Guid fileAccessTokenId)
        {
            return this.FindByIdAsync(fileAccessTokenId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 取得指定 ID 之檔案存取權杖
        /// </summary>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<FileAccessTokenResponse> FindByIdAsync(System.Guid fileAccessTokenId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileaccesstokens/{fileAccessTokenId}");
            urlBuilder_.Replace("{fileAccessTokenId}", System.Uri.EscapeDataString(base.ConvertToString(fileAccessTokenId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                        {
                            headers_[item_.Key] = item_.Value;
                        }
                    }

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await base.ReadObjectResponseAsync<FileAccessTokenResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else
                    if (status_ == 404)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Not Found", status_, responseText_, headers_, null);
                    }
                    else
                    if (status_ == 401)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Unauthorized", status_, responseText_, headers_, null);
                    }
                    else
                    if (status_ == 403)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Forbidden", status_, responseText_, headers_, null);
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    response_.Dispose();
                }
            }
        }

        /// <summary>
        /// 撤銷指定 ID 的權杖
        /// </summary>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<FileAccessTokenResponse> RevokeAsync(System.Guid fileAccessTokenId)
        {
            return this.RevokeAsync(fileAccessTokenId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 撤銷指定 ID 的權杖
        /// </summary>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<FileAccessTokenResponse> RevokeAsync(System.Guid fileAccessTokenId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileaccesstokens/{fileAccessTokenId}/revoke");
            urlBuilder_.Replace("{fileAccessTokenId}", System.Uri.EscapeDataString(base.ConvertToString(fileAccessTokenId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                request_.Method = new System.Net.Http.HttpMethod("PUT");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                        {
                            headers_[item_.Key] = item_.Value;
                        }
                    }

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<FileAccessTokenResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else
                    if (status_ == 404)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Not Found", status_, responseText_, headers_, null);
                    }
                    else
                    if (status_ == 401)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Unauthorized", status_, responseText_, headers_, null);
                    }
                    else
                    if (status_ == 403)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Forbidden", status_, responseText_, headers_, null);
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    response_.Dispose();
                }
            }
        }
    }
}
