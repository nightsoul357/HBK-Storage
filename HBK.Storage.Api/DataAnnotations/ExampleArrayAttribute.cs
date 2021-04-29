using System;
using System.Linq;
using Microsoft.OpenApi.Any;

namespace HBK.Storage.Api.DataAnnotations
{
    /// <summary>
    /// 標示範例參數的標記
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ExampleArrayAttribute : Attribute
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public ExampleArrayAttribute()
        {
            this.Values = new OpenApiString[0];
        }
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="values">範例值</param>
        public ExampleArrayAttribute(params string[] values)
        {
            this.Values = values.Select(value => new OpenApiString(value)).ToArray();
        }
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="values">範例值</param>
        public ExampleArrayAttribute(params int[] values)
        {
            this.Values = values.Select(value => new OpenApiInteger(value)).ToArray();
        }
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="values">範例值</param>
        public ExampleArrayAttribute(params double[] values)
        {
            this.Values = values.Select(value => new OpenApiDouble(value)).ToArray();
        }

        /// <summary>
        /// 取得範例值
        /// </summary>
        public IOpenApiAny[] Values { get; }
    }
}
