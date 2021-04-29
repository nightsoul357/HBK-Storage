using System;
using System.IO;
using Microsoft.Net.Http.Headers;

namespace HBK.Storage.Api.Helpers
{
    /// <summary>
    /// 判斷請求是否為 multipart 輔助類別
    /// </summary>
    public static class MultipartRequestHelper
    {
        /// <summary>
        /// 取得 Boundary
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="lengthLimit"></param>
        /// <returns></returns>
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
            if (string.IsNullOrWhiteSpace(boundary.Value))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");
            }
            return boundary.Value;
        }

        /// <summary>
        /// 判斷請求是否為指定的 ContentType
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 判斷否包含 Form Data Content Disposition
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }
        /// <summary>
        /// 判斷否包含 File Data Content Disposition
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                 || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }
    }
}