using HBK.Storage.SDK.Models;
using HBK.Storage.SDK.Models.Storage;
using HBK.Storage.SDK.Models.StorageGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Services
{
    /// <summary>
    /// 儲存群組服務
    /// </summary>
    public class StorageGroupService : ServiceBase
    {
        public StorageGroupService(string baseUrl, Func<HttpClient> httpClientFunc, string apiKey)
            : base(baseUrl, httpClientFunc, apiKey)
        {
        }

        /// <summary>
        /// 取得指定 ID 之儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<StorageGroupResponse> FindByIdAsync(System.Guid storageGroupId)
        {
            return this.FindByIdAsync(storageGroupId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 取得指定 ID 之儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<StorageGroupResponse> FindByIdAsync(System.Guid storageGroupId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storagegroups/{storageGroupId}");
            urlBuilder_.Replace("{storageGroupId}", System.Uri.EscapeDataString(base.ConvertToString(storageGroupId, System.Globalization.CultureInfo.InvariantCulture)));

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<StorageGroupResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 更新儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="body">更新儲存個體集合請求內容</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<StorageGroupResponse> UpdateAsync(System.Guid storageGroupId, StorageGroupUpdateRequest body)
        {
            return this.UpdateAsync(storageGroupId, body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 更新儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="body">更新儲存個體集合請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<StorageGroupResponse> UpdateAsync(System.Guid storageGroupId, StorageGroupUpdateRequest body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storagegroups/{storageGroupId}");
            urlBuilder_.Replace("{storageGroupId}", System.Uri.EscapeDataString(base.ConvertToString(storageGroupId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body, _settings.Value));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
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
                        var objectResponse_ = await ReadObjectResponseAsync<StorageGroupResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 刪除儲存個體集合(同時會刪除所有儲存個體)
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClient">A server side error occurred.</exception>
        public System.Threading.Tasks.Task DeleteAsync(System.Guid storageGroupId)
        {
            return this.DeleteAsync(storageGroupId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 刪除儲存個體集合(同時會刪除所有儲存個體)
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClient">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task DeleteAsync(System.Guid storageGroupId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storagegroups/{storageGroupId}");
            urlBuilder_.Replace("{storageGroupId}", System.Uri.EscapeDataString(base.ConvertToString(storageGroupId, System.Globalization.CultureInfo.InvariantCulture)));

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
        /// 新增儲存個體
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="body">新增儲存個體請求內容</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<StorageResponse> AddStorageAsync(System.Guid storageGroupId, StorageAddRequest body)
        {
            return this.AddStorageAsync(storageGroupId, body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 新增儲存個體
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="body">新增儲存個體請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<StorageResponse> AddStorageAsync(System.Guid storageGroupId, StorageAddRequest body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storagegroups/{storageGroupId}/storages");
            urlBuilder_.Replace("{storageGroupId}", System.Uri.EscapeDataString(base.ConvertToString(storageGroupId, System.Globalization.CultureInfo.InvariantCulture)));

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<StorageResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 🔧取得儲存群組內的儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`name`, `type`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`name`, `type`, `size_limit`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<PagedResponse<StorageResponse>> GetStoragesAsync(System.Guid storageGroupId, string filter, string orderby, int? skip, int? top)
        {
            return this.GetStoragesAsync(storageGroupId, filter, orderby, skip, top, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 🔧取得儲存群組內的儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`name`, `type`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`name`, `type`, `size_limit`, `create_date_time`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<PagedResponse<StorageResponse>> GetStoragesAsync(System.Guid storageGroupId, string filter, string orderby, int? skip, int? top, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storagegroups/{storageGroupId}/storages?");
            urlBuilder_.Replace("{storageGroupId}", System.Uri.EscapeDataString(base.ConvertToString(storageGroupId, System.Globalization.CultureInfo.InvariantCulture)));
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
                        var objectResponse_ = await base.ReadObjectResponseAsync<PagedResponse<StorageResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 🔧取得儲存群組內的儲存個體擴展資訊集合，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageGroupId">儲存群組 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`storage`, `used_size`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`storage`, `used_size`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<PagedResponse<StorageExtendPropertyResponse>> GetStorageExtendPropertiesAsync(System.Guid storageGroupId, string filter, string orderby, int? skip, int? top)
        {
            return this.GetStorageExtendPropertiesAsync(storageGroupId, filter, orderby, skip, top, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 🔧取得儲存群組內的儲存個體擴展資訊集合，單次資料上限為 100 筆
        /// </summary>
        /// <param name="storageGroupId">儲存群組 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`storage`, `used_size`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`storage`, `used_size`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<PagedResponse<StorageExtendPropertyResponse>> GetStorageExtendPropertiesAsync(System.Guid storageGroupId, string filter, string orderby, int? skip, int? top, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/storagegroups/{storageGroupId}/storageextendproperties?");
            urlBuilder_.Replace("{storageGroupId}", System.Uri.EscapeDataString(base.ConvertToString(storageGroupId, System.Globalization.CultureInfo.InvariantCulture)));
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
                        var objectResponse_ = await base.ReadObjectResponseAsync<PagedResponse<StorageExtendPropertyResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 測試同步策略
        /// </summary>
        /// <param name="body">測試同步策略請求內容</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<TestSyncPolicyResponse> TestPolicyAsync(TestSyncPolicyRequest body)
        {
            return this.TestPolicyAsync(body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 測試同步策略
        /// </summary>
        /// <param name="body">測試同步策略請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<TestSyncPolicyResponse> TestPolicyAsync(TestSyncPolicyRequest body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/storagegroups/testpolicy");

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<TestSyncPolicyResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new HBKStorageClientException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
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
    }
}
