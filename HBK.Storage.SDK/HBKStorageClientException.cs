using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK
{
    /// <summary>
    /// HBK Storage 發生錯誤時擲出的例外
    /// </summary>
    internal class HBKStorageClientException : Exception
    {
        /// <summary>
        /// 取得狀態碼
        /// </summary>
        public int StatusCode { get; private set; }

        /// <summary>
        /// 取得回應內容
        /// </summary>
        public string? Response { get; private set; }

        /// <summary>
        /// 取得標頭
        /// </summary>
        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

        /// <summary>
        /// 初始化 HBK Storage 發生錯誤時擲出的例外
        /// </summary>
        /// <param name="message">訊息</param>
        /// <param name="statusCode">狀態碼</param>
        /// <param name="response">回應內容</param>
        /// <param name="headers">標頭</param>
        /// <param name="innerException">內部例外</param>
        public HBKStorageClientException(string message, int statusCode, string? response, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception? innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
        {
            this.StatusCode = statusCode;
            this.Response = response;
            this.Headers = headers;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    /// <summary>
    /// 具有回傳型別的 HBK Storage 發生錯誤時擲出的例外
    /// </summary>
    /// <typeparam name="TResult">回傳型別</typeparam>
    internal class HBKStorageClientException<TResult> : HBKStorageClientException
    {
        /// <summary>
        /// 取得結果
        /// </summary>
        public TResult Result { get; private set; }

        /// <inheritdoc/>
        public HBKStorageClientException(string message, int statusCode, string response, IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            this.Result = result;
        }
    }
}
