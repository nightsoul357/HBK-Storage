using System;
using Microsoft.OpenApi.Any;

namespace HBK.Storage.Api.DataAnnotations
{
    /// <summary>
    /// 標示範例參數的標記
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class ExampleParameterAttribute : Attribute
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="value">範例值</param>
        public ExampleParameterAttribute(string value)
        {
            this.Value = new OpenApiString(value);
        }
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="value">範例值</param>
        public ExampleParameterAttribute(int value)
        {
            this.Value = new OpenApiInteger(value);
        }
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="value">範例值</param>
        public ExampleParameterAttribute(double value)
        {
            this.Value = new OpenApiDouble(value);
        }

        /// <summary>
        /// 取得範例值
        /// </summary>
        public IOpenApiAny Value { get; }
    }
}
