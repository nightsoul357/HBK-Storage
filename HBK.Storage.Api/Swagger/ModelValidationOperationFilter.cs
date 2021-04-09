using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// 提供 Swagger 標記資料驗證需求
    /// </summary>
    public class ModelValidationOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 套用過濾器
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo?.GetParameters()
                ?.SelectMany(param => param.GetCustomAttributes<FromBodyAttribute>());

            if (attributes.Any())
            {
                operation.Responses.Add("400", new OpenApiResponse { Description = "Bad Request" });
            }
        }
    }
}
