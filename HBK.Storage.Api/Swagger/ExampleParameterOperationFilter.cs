using System.Linq;
using System.Reflection;
using HBK.Storage.Api.DataAnnotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// 提供 Swagger 顯示範例參數
    /// </summary>
    public class ExampleParameterOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 套用過濾器
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo?.GetParameters()
                ?.ToDictionary(param => param.Name, param => param.GetCustomAttribute<ExampleParameterAttribute>());

            foreach (var parameter in operation.Parameters)
            {
                if (attributes.ContainsKey(parameter.Name))
                {
                    ExampleParameterAttribute attribute = attributes[parameter.Name];
                    if (attribute != null)
                    {
                        parameter.Example = attribute.Value;
                    }
                }
            }
        }
    }
}
