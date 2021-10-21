using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.DataAnnotations
{
    /// <summary>
    /// 標示請求 Header 的標記
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HeaderParameterAttribute : Attribute
    {
        /// <summary>
        /// 取得或設定名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 取得或設定是否為必要
        /// </summary>
        public bool Required { get; set; } = false;
        /// <summary>
        /// 取得或設定說明
        /// </summary>
        public string Description { get; set; }
    }
}
