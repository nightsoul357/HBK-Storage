using System.Reflection;
using HBK.Storage.Api.DataAnnotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// 提供特殊的範例值
    /// </summary>
    public class ExampleValueSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// 套用過濾器
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            SnakeCaseNamingStrategy snakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

            foreach (var property in context.Type.GetProperties())
            {
                ExampleArrayAttribute exampleArrayAttribute = property.GetCustomAttribute<ExampleArrayAttribute>();

                if (exampleArrayAttribute != null && property.PropertyType.IsArray)
                {
                    var array = new OpenApiArray();
                    array.AddRange(exampleArrayAttribute.Values);

                    string propertyName = snakeCaseNamingStrategy.GetPropertyName(property.Name, false);
                    if (schema.Properties[propertyName].Items?.Reference == null)
                    {
                        schema.Properties[propertyName].Example = array;
                    }
                    else
                    {
                        schema.Properties[propertyName].Default = array;
                    }
                }
            }
        }
    }
}
