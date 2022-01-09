using HBK.Storage.SDK.Models;
using HBK.Storage.SDK.Models.FileEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Services
{
    /// <summary>
    /// 檔案服務
    /// </summary>
    public class FileEntityService : ServiceBase
    {
        public FileEntityService(string baseUrl, Func<HttpClient> httpClientFunc, string apiKey)
            : base(baseUrl, httpClientFunc, apiKey)
        {
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="storageGroupId">強制指定儲存個體群組 ID</param>
        /// <param name="handlerIndicate">處理器指示字串</param>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<FileResponse> GetFileResponseAsync(System.Guid fileEntityId, System.Guid? storageGroupId, string handlerIndicate)
        {
            return this.GetFileResponseAsync(fileEntityId, storageGroupId, handlerIndicate, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="storageGroupId">強制指定儲存個體群組 ID</param>
        /// <param name="handlerIndicate">處理器指示字串</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<FileResponse> GetFileResponseAsync(System.Guid fileEntityId, System.Guid? storageGroupId, string handlerIndicate, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileentities/{fileEntityId}?");
            urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));
            if (storageGroupId != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("storageGroupId") + "=").Append(System.Uri.EscapeDataString(base.ConvertToString(storageGroupId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (handlerIndicate != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("handlerIndicate") + "=").Append(System.Uri.EscapeDataString(base.ConvertToString(handlerIndicate, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = base.GetHttpClient();
            var disposeClient_ = true;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/octet-stream"));

                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
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
                        if (status_ == 200 || status_ == 206)
                        {
                            var responseStream_ = response_.Content == null ? System.IO.Stream.Null : await response_.Content.ReadAsStreamAsync().ConfigureAwait(false);
                            var fileResponse_ = new FileResponse(status_, headers_, responseStream_, client_, response_);
                            disposeClient_ = false; disposeResponse_ = false; // response and client are disposed by FileResponse
                            return fileResponse_;
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
                        if (disposeResponse_)
                        {
                            response_.Dispose();
                        }
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                {
                    client_.Dispose();
                }
            }
        }

        /// <summary>
        /// 更新檔案實體
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="body">更新檔案實體請求內容</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<FileEntityResponse> UpdateAsync(System.Guid fileEntityId, FileEntityUpdateRequest body)
        {
            return this.UpdateAsync(fileEntityId, body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 更新檔案實體
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="body">更新檔案實體請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<FileEntityResponse> UpdateAsync(System.Guid fileEntityId, FileEntityUpdateRequest body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileentities/{fileEntityId}");
            urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));

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
                var disposeResponse_ = true;
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
                        var objectResponse_ = await base.ReadObjectResponseAsync<FileEntityResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
                    if (disposeResponse_)
                    {
                        response_.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// 將檔案實體標記為刪除
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task DeleteAsync(System.Guid fileEntityId)
        {
            return this.DeleteAsync(fileEntityId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 將檔案實體標記為刪除
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task DeleteAsync(System.Guid fileEntityId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileentities/{fileEntityId}");
            urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));

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
        /// 取得檔案的所有存取權杖資訊，單次資料上限為 100 筆
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<PagedResponse<FileAccessTokenResponse>> GetFileAccessTokensAsync(System.Guid fileEntityId)
        {
            return this.GetFileAccessTokensAsync(fileEntityId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 取得檔案的所有存取權杖資訊，單次資料上限為 100 筆
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<PagedResponse<FileAccessTokenResponse>> GetFileAccessTokensAsync(System.Guid fileEntityId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileentities/{fileEntityId}/fileaccesstokens");
            urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<PagedResponse<FileAccessTokenResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 🔧取得指定檔案的所有子檔案(遞迴查詢)
        /// </summary>
        /// <param name="fileEntityId">檔案 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`file_entity`, `child_level`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`file_entity`, `child_level`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量，上限 100</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<PagedResponse<ChildFileEntityResponse>> GetChildsAsync(System.Guid fileEntityId, string filter, string orderby, int? skip, int? top)
        {
            return this.GetChildsAsync(fileEntityId, filter, orderby, skip, top, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 🔧取得指定檔案的所有子檔案(遞迴查詢)
        /// </summary>
        /// <param name="fileEntityId">檔案 ID</param>
        /// <param name="filter">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System) 篩選指定欄位
        /// <br/>* 允許的欄位：`file_entity`, `child_level`
        /// <br/>
        /// <br/>
        /// <br/>* 允許的函數：`contains()`, `cast()`, `any()`, `all()`</param>
        /// <param name="orderby">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1) 指定結果依照指定欄位排序
        /// <br/>* 允許的欄位：`file_entity`, `child_level`</param>
        /// <param name="skip">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System) 指定要跳過的資料數量</param>
        /// <param name="top">[OData v4](http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1) 指定要取得的資料數量，上限 100</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<PagedResponse<ChildFileEntityResponse>> GetChildsAsync(System.Guid fileEntityId, string filter, string orderby, int? skip, int? top, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileentities/{fileEntityId}/childs?");
            urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));
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
                        var objectResponse_ = await base.ReadObjectResponseAsync<PagedResponse<ChildFileEntityResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 附加檔案實體標籤
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="body">附加檔案實體標籤請求內容</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<FileEntityResponse> AppendTagAsync(System.Guid fileEntityId, AppendTagRequest body)
        {
            return this.AppendTagAsync(fileEntityId, body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 附加檔案實體標籤
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="body">附加檔案實體標籤請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<FileEntityResponse> AppendTagAsync(System.Guid fileEntityId, AppendTagRequest body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileentities/{fileEntityId}/tag");
            urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));

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
        /// 刪除檔案實體標籤
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="body">刪除檔案實體標籤請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<FileEntityResponse> RemoveTagAsync(System.Guid fileEntityId, DeleteTagRequest body)
        {
            return this.RemoveTagAsync(fileEntityId, body, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 刪除檔案實體標籤
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="body">刪除檔案實體標籤請求內容</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<FileEntityResponse> RemoveTagAsync(System.Guid fileEntityId, DeleteTagRequest body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileentities/{fileEntityId}/tag");
            urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));

            using (var client_ = base.GetHttpClient())
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body, _settings.Value));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("DELETE");
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
                        var objectResponse_ = await base.ReadObjectResponseAsync<FileEntityResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
        /// 取得檔案實體存取次數
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<GetAccessTimesResponse> GetAccessTimesAsync(System.Guid fileEntityId)
        {
            return this.GetAccessTimesAsync(fileEntityId, System.Threading.CancellationToken.None);
        }

        /// <summary>
        /// 取得檔案實體存取次數
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="HBKStorageClientException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<GetAccessTimesResponse> GetAccessTimesAsync(System.Guid fileEntityId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(base.BaseUrl != null ? base.BaseUrl.TrimEnd('/') : "").Append("/fileentities/{fileEntityId}/access_times");
            urlBuilder_.Replace("{fileEntityId}", System.Uri.EscapeDataString(base.ConvertToString(fileEntityId, System.Globalization.CultureInfo.InvariantCulture)));

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
                        var objectResponse_ = await base.ReadObjectResponseAsync<GetAccessTimesResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
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
