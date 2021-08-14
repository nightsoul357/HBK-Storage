using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// 提供 Swagger 標記檔案類型回傳
    /// </summary>
    public class FileStreamTypeOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 套用過濾器
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.ReturnType != typeof(Task<FileStreamResult>) &&
                context.MethodInfo.ReturnType != typeof(Task<ActionResult<FileStreamResult>>))
            {
                return;
            }

            if (operation.Responses.ContainsKey("200"))
            {
                operation.Responses.Remove("200");
            }

            operation.Responses.Add("200", new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/octet-stream", new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        }
                    }
                }
            });
        }
    }
}
