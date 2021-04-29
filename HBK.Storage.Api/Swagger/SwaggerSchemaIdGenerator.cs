using System;
using System.Linq;


namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// 產生 Swagger 的 Schema ID
    /// </summary>
    public static class SwaggerSchemaIdGenerator
    {
        /// <summary>
        /// Schema ID 選擇器
        /// </summary>
        public static Func<Type, string> SchemaIdSelector => (modelType) =>
        {
            // 移除 Enum 結尾
            if (modelType.IsEnum)
            {
                string name = modelType.Name;
                if (name.EndsWith("Enum"))
                {
                    name = name[0..^4];
                }
                return "Enums." + name;
            }

            return SwaggerSchemaIdGenerator.DefaultSchemaIdSelector(modelType);
        };


        /// <summary>
        /// 預設的 Schema ID 選擇器
        /// </summary>
        /// <param name="modelType">模型類型</param>
        /// <returns></returns>
        private static string DefaultSchemaIdSelector(Type modelType)
        {
            if (!modelType.IsConstructedGenericType)
            {
                return modelType.Name.Replace("[]", "Array");
            }

            string prefix = modelType.GetGenericArguments()
                .Select(genericArg => SwaggerSchemaIdGenerator.DefaultSchemaIdSelector(genericArg))
                .Aggregate((previous, current) => previous + current);

            return prefix + modelType.Name.Split('`').First();
        }
    }
}
