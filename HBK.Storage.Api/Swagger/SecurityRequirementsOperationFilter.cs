using System.Collections.Generic;
using System.Linq;
using HBK.Storage.Api.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// 提供 Swagger 標記登入驗證需求
    /// </summary>
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 套用過濾器
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.GetCustomAttributes(true)
                .Concat(context.MethodInfo.DeclaringType.GetCustomAttributes(true));

            foreach (object attribute in attributes)
            {
                if (attribute is AllowAnonymousAttribute)
                {
                    // 允許匿名存取
                    return;
                }
                else if (attribute is TypeFilterAttribute && ((TypeFilterAttribute)attribute).ImplementationType.Name == nameof(HBKAuthorizeFilter))
                {
                    // 需要授權的 API
                    if (!operation.Responses.ContainsKey("401"))
                    {
                        operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                    }
                    if (!operation.Responses.ContainsKey("403"))
                    {
                        operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                    }

                    var apiKeyScheme = new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                    };

                    operation.Security.Add(new OpenApiSecurityRequirement()
                    {
                        {
                            apiKeyScheme,
                            new string[] { }
                        }
                    });
                    return;
                }
            }
        }
    }
}
