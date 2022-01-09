using HBK.Storage.SDK.Models;
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
    /// 服務基底型別
    /// </summary>
    public abstract class ServiceBase
    {
        protected string _baseUrl = "";
        protected Func<HttpClient> _httpClientFunc;
        protected string _apiKey = "";
        protected Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;

        /// <summary>
        /// 初始化服務基底型別服務
        /// </summary>
        /// <param name="baseUrl">基礎 URL</param>
        /// <param name="httpClientFunc">取得 Http Client 方法</param>
        public ServiceBase(string baseUrl, Func<HttpClient> httpClientFunc)
        {
            _baseUrl = baseUrl;
            _httpClientFunc = httpClientFunc;
            _settings = new Lazy<Newtonsoft.Json.JsonSerializerSettings>(this.CreateSerializerSettings);
        }

        /// <summary>
        /// 初始化服務基底型別服務
        /// </summary>
        /// <param name="baseUrl">基礎 Url</param>
        /// <param name="httpClientFunc">取得 Http Client 方法</param>
        /// <param name="apiKey">API Key</param>
        public ServiceBase(string baseUrl, Func<HttpClient> httpClientFunc, string apiKey)
        {
            _baseUrl = baseUrl;
            _httpClientFunc = httpClientFunc;
            _apiKey = apiKey;
            _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(this.CreateSerializerSettings);

        }

        protected Newtonsoft.Json.JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            return settings;
        }

        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }

            try
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new System.IO.StreamReader(responseStream))
                using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                {
                    var serializer = Newtonsoft.Json.JsonSerializer.Create(this.JsonSerializerSettings);
                    var typedBody = serializer.Deserialize<T>(jsonTextReader);
                    return new ObjectResponseResult<T>(typedBody, string.Empty);
                }
            }
            catch (Newtonsoft.Json.JsonException exception)
            {
                var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                throw new HBKStorageClientException(message, (int)response.StatusCode, string.Empty, headers, exception);
            }

        }
        protected HttpClient GetHttpClient()
        {
            var httpClient = _httpClientFunc.Invoke();
            if (string.IsNullOrEmpty(_apiKey))
            {
                httpClient.DefaultRequestHeaders.Add("HBKey", _apiKey);
            }
            return httpClient;
        }
        protected string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }

            if (value is Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute))
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }

                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool)
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[])value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((Array)value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => this.ConvertToString(o, cultureInfo)));
            }

            string? result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }

        protected Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                return _settings.Value;
            }
        }

        /// <summary>
        /// 取得或設定基礎 URL
        /// </summary>
        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }
    }
}
