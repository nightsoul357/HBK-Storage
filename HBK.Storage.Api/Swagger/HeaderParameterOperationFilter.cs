using HBK.Storage.Api.DataAnnotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// 提供 Swagger 顯示 Header 參數
    /// </summary>
    public class HeaderParameterOperationFilter : IOperationFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var headers = context.ApiDescription.ActionDescriptor.EndpointMetadata.Where(x => x is HeaderParameterAttribute)
                .Cast<HeaderParameterAttribute>()
                .ToList();

            if (headers.Count == 0)
            {
                return;
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            foreach (var header in headers)
            {
                var parmeter = new OpenApiParameter()
                {
                    Name = header.Name,
                    In = ParameterLocation.Header,
                    Required = header.Required,
                    Description = header.Description
                };
                operation.Parameters.Add(parmeter);
            }
        }
    }
}
