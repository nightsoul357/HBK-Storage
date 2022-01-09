using HBK.Storage.SDK.Enums;
using HBK.Storage.SDK.Models;
using HBK.Storage.SDK.Models.FileEntity;
using HBK.Storage.SDK.Models.StorageGroup;
using HBK.Storage.SDK.Models.StorageProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Services
{
    /// <summary>
    /// 儲存服務
    /// </summary>
    public class StorageProviderService : ServiceBase
    {
        public StorageProviderService(string baseUrl, Func<HttpClient> httpClientFunc, string apiKey)
            : base(baseUrl, httpClientFunc, apiKey)
        {
        }

        /// <summary>
        /// 取得指定 ID 之儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<StorageProviderResponse> FindByIdAsync(Guid storageProviderId)
        {
            return this.FindByIdAsync(storageProviderId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 取得指定 ID 之儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task<StorageProviderResponse> FindByIdAsync(Guid storageProviderId, CancellationToken cancellationToken)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storageproviders/{storageProviderId}");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(base.ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new HttpRequestMessage())
            {
                request_.Method = new HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, System.UriKind.RelativeOrAbsolute);

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<StorageProviderResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else if (status_ == 404)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Not Found", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 401)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Unauthorized", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 403)
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
        /// 加入儲存服務
        /// </summary>
        /// <param name="body">加入儲存服務請求內容</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<StorageProviderResponse> AddAsync(StorageProviderAddRequest body)
        {
            return this.AddAsync(body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 加入儲存服務
        /// </summary>
        /// <param name="body">加入儲存服務請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task<StorageProviderResponse> AddAsync(StorageProviderAddRequest body, CancellationToken cancellationToken)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storageproviders");

            using (var client_ = base.GetHttpClient())
            using (var request_ = new HttpRequestMessage())
            {
                var content_ = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body, base._settings.Value));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new HttpMethod("POST");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, System.UriKind.RelativeOrAbsolute);

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
                        var objectResponse_ = await ReadObjectResponseAsync<StorageProviderResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else if (status_ == 401)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Unauthorized", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 403)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Forbidden", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 400)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Bad Request", status_, responseText_, headers_, null);
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
        /// 更新儲存服務內容
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="body">更新儲存服務內容請求內容</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<StorageProviderResponse> UpdateAsync(Guid storageProviderId, StorageProviderUpdateRequest body)
        {
            return this.UpdateAsync(storageProviderId, body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 更新儲存服務內容
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="body">更新儲存服務內容請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task<StorageProviderResponse> UpdateAsync(Guid storageProviderId, StorageProviderUpdateRequest body, CancellationToken cancellationToken)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storageproviders/{storageProviderId}");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(base.ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new HttpRequestMessage())
            {
                var content_ = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body, base._settings.Value));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new HttpMethod("PUT");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, System.UriKind.RelativeOrAbsolute);

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<StorageProviderResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else if (status_ == 404)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Not Found", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 401)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Unauthorized", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 403)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Forbidden", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 400)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Bad Request", status_, responseText_, headers_, null);
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
        /// 刪除儲存服務(包含所有儲存個體集合)
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task DeleteAsync(Guid storageProviderId)
        {
            return this.DeleteAsync(storageProviderId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 刪除儲存服務(包含所有儲存個體集合)
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task DeleteAsync(Guid storageProviderId, CancellationToken cancellationToken)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storageproviders/{storageProviderId}");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(base.ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new HttpRequestMessage())
            {
                request_.Method = new HttpMethod("DELETE");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, System.UriKind.RelativeOrAbsolute);

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
                    else if (status_ == 404)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Not Found", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 401)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Unauthorized", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 403)
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
        /// 🔧取得所有儲存服務資訊
        /// </summary>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`name`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`name`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<PagedResponse<StorageProviderResponse>> ListAsync(string filter, string orderby, int? skip, int? top)
        {
            return this.ListAsync(filter, orderby, skip, top, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 🔧取得所有儲存服務資訊
        /// </summary>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`name`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`name`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task<PagedResponse<StorageProviderResponse>> ListAsync(string filter, string orderby, int? skip, int? top, CancellationToken cancellationToken)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storageproviders?");
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
            using (var request_ = new HttpRequestMessage())
            {
                request_.Method = new HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, System.UriKind.RelativeOrAbsolute);

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<PagedResponse<StorageProviderResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else if (status_ == 401)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Unauthorized", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 403)
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
        /// 新增儲存個體集合
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="body">新增儲存個體集合請求內容</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<StorageGroupResponse> AddStorageGroupAsync(Guid storageProviderId, StorageGroupAddRequest body)
        {
            return this.AddStorageGroupAsync(storageProviderId, body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 新增儲存個體集合
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="body">新增儲存個體集合請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task<StorageGroupResponse> AddStorageGroupAsync(Guid storageProviderId, StorageGroupAddRequest body, CancellationToken cancellationToken)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storageproviders/{storageProviderId}/storagegroups");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(base.ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new HttpRequestMessage())
            {
                var content_ = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body, base._settings.Value));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new HttpMethod("POST");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, System.UriKind.RelativeOrAbsolute);

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
                        var objectResponse_ = await ReadObjectResponseAsync<StorageGroupResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else if (status_ == 404)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Not Found", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 401)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Unauthorized", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 403)
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
        /// 🔧取得儲存服務內的所有儲存群組集合
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`name`, `type`, `sync_mode`, `upload_priority`, `download_priority`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`name`, `type`, `sync_mode`, `upload_priority`, `download_priority`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<PagedResponse<StorageGroupResponse>> GetStorgaeGroupsAsync(Guid storageProviderId, string filter, string orderby, int? skip, int? top)
        {
            return this.GetStorgaeGroupsAsync(storageProviderId, filter, orderby, skip, top, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 🔧取得儲存服務內的所有儲存群組集合
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`name`, `type`, `sync_mode`, `upload_priority`, `download_priority`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`name`, `type`, `sync_mode`, `upload_priority`, `download_priority`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task<PagedResponse<StorageGroupResponse>> GetStorgaeGroupsAsync(Guid storageProviderId, string filter, string orderby, int? skip, int? top, CancellationToken cancellationToken)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/storageproviders/{storageProviderId}/storagegroups?");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));
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
            using (var request_ = new HttpRequestMessage())
            {
                request_.Method = new HttpMethod("GET");
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
                        var objectResponse_ = await ReadObjectResponseAsync<PagedResponse<StorageGroupResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else if (status_ == 404)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Not Found", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 401)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Unauthorized", status_, responseText_, headers_, null);
                    }
                    else if (status_ == 403)
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
        /// 🔧取得儲存服務內的所有儲存群組擴充資訊集合，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`storage_group`, `size_limit`, `used_size`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`storage_group`, `size_limit`, `used_size`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<PagedResponse<StorageGroupExtendPropertyResponse>> GetStorageGroupExtendPropertiesAsync(System.Guid storageProviderId, string filter, string orderby, int? skip, int? top)
        {
            return this.GetStorageGroupExtendPropertiesAsync(storageProviderId, filter, orderby, skip, top, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 🔧取得儲存服務內的所有儲存群組擴充資訊集合，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`storage_group`, `size_limit`, `used_size`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`storage_group`, `size_limit`, `used_size`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task<PagedResponse<StorageGroupExtendPropertyResponse>> GetStorageGroupExtendPropertiesAsync(System.Guid storageProviderId, string filter, string orderby, int? skip, int? top, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storageproviders/{storageProviderId}/storagegroupextendproperties?");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(base.ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));
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
                        var objectResponse_ = await ReadObjectResponseAsync<PagedResponse<StorageGroupExtendPropertyResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 🔧取得儲存服務內的檔案實體集合，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`file_entity_id`, `name`, `size`, `extend_property`, `mime_type`, `parent_file_entity_id`, `create_date_time`, `file_entity_tag`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`name`, `size`, `mime_type`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量，上限 100</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<PagedResponse<FileEntityResponse>> GetFileEntitiesAsync(System.Guid storageProviderId, string filter, string orderby, int? skip, int? top)
        {
            return this.GetFileEntitiesAsync(storageProviderId, filter, orderby, skip, top, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 🔧取得儲存服務內的檔案實體集合，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`file_entity_id`, `name`, `size`, `extend_property`, `mime_type`, `parent_file_entity_id`, `create_date_time`, `file_entity_tag`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`name`, `size`, `mime_type`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量，上限 100</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task<PagedResponse<FileEntityResponse>> GetFileEntitiesAsync(System.Guid storageProviderId, string filter, string orderby, int? skip, int? top, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/storageproviders/{storageProviderId}/fileentities?");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(base.ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));
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
                        var objectResponse_ = await ReadObjectResponseAsync<PagedResponse<FileEntityResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 上傳檔案至指定的儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<FileEntityResponse> UploadFileAsync(System.Guid storageProviderId, System.Guid? storageGroupId, string extendProperty, System.Collections.Generic.IEnumerable<string> tags, CryptoMode? cryptoMode, FileParameter file)
        {
            return this.UploadFileAsync(storageProviderId, storageGroupId, extendProperty, tags, cryptoMode, file);
        }

        /// <summary>
        /// 上傳檔案至指定的儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<FileEntityResponse> UploadFileAsync(System.Guid storageProviderId, System.Guid? storageGroupId, string extendProperty, System.Collections.Generic.IEnumerable<string> tags, CryptoMode? cryptoMode, FileParameter file, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storageproviders/{storageProviderId}/fileentities");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(base.ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var boundary_ = System.Guid.NewGuid().ToString();
                var content_ = new System.Net.Http.MultipartFormDataContent(boundary_);
                content_.Headers.Remove("Content-Type");
                content_.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary_);
                if (storageGroupId != null)
                {
                    content_.Add(new System.Net.Http.StringContent(base.ConvertToString(storageGroupId, System.Globalization.CultureInfo.InvariantCulture)), "StorageGroupId");
                }
                if (extendProperty != null)
                {
                    content_.Add(new System.Net.Http.StringContent(base.ConvertToString(extendProperty, System.Globalization.CultureInfo.InvariantCulture)), "ExtendProperty");
                }
                if (tags != null)
                {
                    content_.Add(new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(tags)), "Tags");
                }
                if (cryptoMode != null)
                {
                    content_.Add(new System.Net.Http.StringContent(base.ConvertToString(cryptoMode, System.Globalization.CultureInfo.InvariantCulture)), "CryptoMode");
                }
                if (file == null)
                {
                    throw new System.ArgumentNullException("file");
                }
                else
                {
                    var content_file_ = new System.Net.Http.StreamContent(file.Data);
                    if (!string.IsNullOrEmpty(file.ContentType))
                    {
                        content_file_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(file.ContentType);
                    }
                    content_.Add(content_file_, "File", file.FileName ?? "File");
                }
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
                        var objectResponse_ = await ReadObjectResponseAsync<FileEntityResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 產生檔案存取權杖
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public Task<PostFileAccessTokenResponse> GenerateFileAccessTokenAsync(System.Guid storageProviderId, PostFileAccessTokenRequest body)
        {
            return this.GenerateFileAccessTokenAsync(storageProviderId, body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 產生檔案存取權杖
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async Task<PostFileAccessTokenResponse> GenerateFileAccessTokenAsync(System.Guid storageProviderId, PostFileAccessTokenRequest body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/storageproviders/{storageProviderId}/fileaccesstokens");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(base.ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body, base._settings.Value));
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
                        var objectResponse_ = await base.ReadObjectResponseAsync<PostFileAccessTokenResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
                    if (status_ == 400)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new HBKStorageClientException("Bad Request", status_, responseText_, headers_, null);
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
        /// 🔧取得檔案存取權杖清單，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`access_times_limit`, `access_times`, `status`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`access_times_limit`, `access_times`, `expire_date_time`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量，上限 100</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<PagedResponse<FileAccessTokenResponse>> GetFileAccessTokensAsync(System.Guid storageProviderId, string filter, string orderby, int? skip, int? top)
        {
            return this.GetFileAccessTokensAsync(storageProviderId, filter, orderby, skip, top, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 🔧取得檔案存取權杖清單，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`access_times_limit`, `access_times`, `status`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`access_times_limit`, `access_times`, `expire_date_time`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量，上限 100</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<PagedResponse<FileAccessTokenResponse>> GetFileAccessTokensAsync(System.Guid storageProviderId, string filter, string orderby, int? skip, int? top, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/{storageProviderId}/fileaccesstokens?");
            urlBuilder_.Replace("{storageProviderId}", System.Uri.EscapeDataString(base.ConvertToString(storageProviderId, System.Globalization.CultureInfo.InvariantCulture)));
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
                        var objectResponse_ = await ReadObjectResponseAsync<PagedResponse<FileAccessTokenResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
