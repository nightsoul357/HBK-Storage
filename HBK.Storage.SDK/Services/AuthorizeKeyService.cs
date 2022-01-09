using HBK.Storage.SDK.Models;
using HBK.Storage.SDK.Models.AuthorizeKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Services
{
    /// <summary>
    /// 驗證金鑰服務
    /// </summary>
    public class AuthorizeKeyService : ServiceBase
    {
        public AuthorizeKeyService(string baseUrl, Func<HttpClient> httpClientFunc, string apiKey)
            : base(baseUrl, httpClientFunc, apiKey)
        {
        }

        /// <summary>
        /// 取得指定 ID 驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<AuthorizeKeyResponse> FindByIdAsync(System.Guid authorizeKeyId)
        {
            return this.FindByIdAsync(authorizeKeyId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 取得指定 ID 驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<AuthorizeKeyResponse> FindByIdAsync(System.Guid authorizeKeyId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/authorizekey/{authorizeKeyId}");
            urlBuilder_.Replace("{authorizeKeyId}", System.Uri.EscapeDataString(base.ConvertToString(authorizeKeyId, System.Globalization.CultureInfo.InvariantCulture)));

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<AuthorizeKeyResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 刪除驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task DeleteAsync(System.Guid authorizeKeyId)
        {
            return this.DeleteAsync(authorizeKeyId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 刪除驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task DeleteAsync(System.Guid authorizeKeyId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/authorizekey/{authorizeKeyId}");
            urlBuilder_.Replace("{authorizeKeyId}", System.Uri.EscapeDataString(base.ConvertToString(authorizeKeyId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("DELETE");

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
                    if (status_ == 204)
                    {
                        return;
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
        /// 取得指定金鑰值的驗證金鑰(無須驗證)
        /// </summary>
        /// <param name="keyValue">金鑰值</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<AuthorizeKeyResponse> GetAuthorizeKeyByKeyValueAsync(string keyValue)
        {
            return this.GetAuthorizeKeyByKeyValueAsync(keyValue, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 取得指定金鑰值的驗證金鑰(無須驗證)
        /// </summary>
        /// <param name="keyValue">金鑰值</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<AuthorizeKeyResponse> GetAuthorizeKeyByKeyValueAsync(string keyValue, System.Threading.CancellationToken cancellationToken)
        {
            if (keyValue == null)
            {
                throw new System.ArgumentNullException("keyValue");
            }

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/authorizekey/key/{keyValue}");
            urlBuilder_.Replace("{keyValue}", System.Uri.EscapeDataString(base.ConvertToString(keyValue, System.Globalization.CultureInfo.InvariantCulture)));

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<AuthorizeKeyResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 新增驗證金鑰
        /// </summary>
        /// <param name="body">新增驗證金鑰請求內容</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<AuthorizeKeyResponse> AddAsync(AddAuthorizeKeyRequest body)
        {
            return this.AddAsync(body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 新增驗證金鑰
        /// </summary>
        /// <param name="body">新增驗證金鑰請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<AuthorizeKeyResponse> AddAsync(AddAuthorizeKeyRequest body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/authorizekey");

            using (var client_ = base.GetHttpClient())
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body, _settings.Value));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");
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
                        var objectResponse_ = await base.ReadObjectResponseAsync<AuthorizeKeyResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
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
        /// 🔧取得所有驗證金鑰，單次資料上限為 100 筆
        /// </summary>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量，上限 100</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<PagedResponse<AuthorizeKeyResponse>> List(string filter, string orderby, int? skip, int? top)
        {
            return this.List(filter, orderby, skip, top, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 🔧取得所有驗證金鑰，單次資料上限為 100 筆
        /// </summary>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量，上限 100</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<PagedResponse<AuthorizeKeyResponse>> List(string filter, string orderby, int? skip, int? top, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/authorizekey?");
            if (filter != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("$filter") + "=").Append(System.Uri.EscapeDataString(base.ConvertToString(filter, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (orderby != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("$orderby") + "=").Append(System.Uri.EscapeDataString(base.ConvertToString(orderby, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("$skip") + "=").Append(System.Uri.EscapeDataString(base.ConvertToString(skip, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (top != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("$top") + "=").Append(System.Uri.EscapeDataString(base.ConvertToString(top, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<PagedResponse<AuthorizeKeyResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
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
        /// 停用驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<AuthorizeKeyResponse> DisableAsync(System.Guid authorizeKeyId)
        {
            return this.DisableAsync(authorizeKeyId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 停用驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<AuthorizeKeyResponse> DisableAsync(System.Guid authorizeKeyId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/authorizekey/{authorizeKeyId}/disable");
            urlBuilder_.Replace("{authorizeKeyId}", System.Uri.EscapeDataString(base.ConvertToString(authorizeKeyId, System.Globalization.CultureInfo.InvariantCulture)));

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<AuthorizeKeyResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 啟用驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<AuthorizeKeyResponse> EnableAsync(System.Guid authorizeKeyId)
        {
            return this.EnableAsync(authorizeKeyId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 啟用驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<AuthorizeKeyResponse> EnableAsync(System.Guid authorizeKeyId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/authorizekey/{authorizeKeyId}/enable");
            urlBuilder_.Replace("{authorizeKeyId}", System.Uri.EscapeDataString(base.ConvertToString(authorizeKeyId, System.Globalization.CultureInfo.InvariantCulture)));

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<AuthorizeKeyResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
