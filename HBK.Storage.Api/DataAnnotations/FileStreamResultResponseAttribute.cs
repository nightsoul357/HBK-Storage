using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.DataAnnotations
{
    /// <summary>
    /// 標記方法以 <see cref="FileStreamResult"/> 回傳檔案
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class FileStreamResultResponseAttribute : Attribute
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public FileStreamResultResponseAttribute()
        {
            this.ContentType = "application/octet-stream";
        }
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="contentType">類型</param>
        public FileStreamResultResponseAttribute(string contentType)
        {
            this.ContentType = contentType;
        }

        /// <summary>
        /// 取得類型
        /// </summary>
        public string ContentType { get; }
    }
}
