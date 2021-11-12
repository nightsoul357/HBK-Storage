using HBK.Storage.Api.Helpers;
using HBK.Storage.Api.Models.FileService;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Api.ModelBinders
{
    /// <summary>
    /// PutFileRequest 的資料綁定
    /// </summary>
    public class PutFileRequestBinder : IModelBinder
    {
        private readonly FormOptions _defaultFormOptions = new FormOptions();
        /// <inheritdoc/>
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (!MultipartRequestHelper.IsMultipartContentType(bindingContext.HttpContext.Request.ContentType))
            {
                throw new Exception($"Expected a multipart request, but got {bindingContext.HttpContext.Request.ContentType}");
            }

            var model = new PutFileRequest();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(bindingContext.HttpContext.Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, bindingContext.HttpContext.Request.Body);

            MultipartSection section = await reader.ReadNextSectionAsync();
            int valueCount = 0;
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        model.FileStream = section.Body;
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            string value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            var prop = model.GetType().GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (prop != null && !string.IsNullOrEmpty(value))
                            {
                                if (prop.PropertyType == typeof(String))
                                {
                                    prop.SetValue(model, value);
                                }
                                else if (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Guid?))
                                {
                                    prop.SetValue(model, Guid.Parse(value));
                                }
                                else
                                {
                                    prop.SetValue(model, JsonConvert.DeserializeObject(value, prop.PropertyType));
                                }
                            }

                            valueCount++;
                            if (valueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                if (model.FileStream != null)
                {
                    break;
                }
                section = await reader.ReadNextSectionAsync();
            }

            if (String.IsNullOrWhiteSpace(model.MimeType))
            {
                model.MimeType = section.ContentType;
            }
            if (String.IsNullOrWhiteSpace(model.MimeType)) // 如果仍然是空值
            {
                model.MimeType = "application/octet-stream";
            }
            if (String.IsNullOrWhiteSpace(model.Filename))
            {
                model.Filename = section.AsFileSection().FileName;
            }
            bindingContext.Result = ModelBindingResult.Success(model);
            return;
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }
}
