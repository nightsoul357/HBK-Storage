using System;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// 提供旗標列舉類別的 Schema 修改
    /// </summary>
    public class FlagEnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// 套用過濾器
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum && context.Type.GetCustomAttribute<FlagsAttribute>() != null)
            {
                var item = schema.Enum
                    .OfType<OpenApiString>()
                    .FirstOrDefault(item => item.Value.Equals("none", StringComparison.OrdinalIgnoreCase));

                if (item != null)
                {
                    schema.Enum.Remove(item);
                }
            }
        }
    }
}
