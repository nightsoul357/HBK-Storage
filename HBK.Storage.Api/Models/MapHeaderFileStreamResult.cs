using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace HBK.Storage.Api.Models
{
    /// <summary>
    /// 附加對應 Header 的 File Stream Result
    /// </summary>
    public class MapHeaderFileStreamResult : FileStreamResult
    {
        private readonly string _overwriteFilename;
        /// <inheritdoc/>
        public MapHeaderFileStreamResult(Stream fileStream, string contentType, string overwriteFilename)
            : base(fileStream, contentType)
        {
            _overwriteFilename = overwriteFilename;
            base.EnableRangeProcessing = true;
        }

        /// <inheritdoc/>
        public MapHeaderFileStreamResult(Stream fileStream, string contentType)
            : base(fileStream, contentType)
        {
            base.EnableRangeProcessing = true;
        }

        /// <inheritdoc/>
        public override Task ExecuteResultAsync(ActionContext context)
        {
            ContentDispositionHeaderValue contentDispositionHeader;
            if (context.HttpContext.Request.Headers.ContainsKey(HeaderNames.ContentDisposition))
            {
                contentDispositionHeader = ContentDispositionHeaderValue.Parse(context.HttpContext.Request.Headers[HeaderNames.ContentDisposition].ToString());
            }
            else
            {
                contentDispositionHeader = new ContentDispositionHeaderValue("inline");
            }

            if (!String.IsNullOrEmpty(base.FileDownloadName) && String.IsNullOrEmpty(contentDispositionHeader.FileName.ToString()))
            {
                contentDispositionHeader.SetHttpFileName(base.FileDownloadName);
            }
            base.FileDownloadName = null;

            if (!String.IsNullOrEmpty(_overwriteFilename))
            {
                contentDispositionHeader.SetHttpFileName(_overwriteFilename);
            }
            context.HttpContext.Response.Headers.Add(HeaderNames.ContentDisposition, contentDispositionHeader.ToString());
            return base.ExecuteResultAsync(context);
        }
    }
}
