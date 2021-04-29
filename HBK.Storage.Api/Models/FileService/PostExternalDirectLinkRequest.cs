using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileService
{
    /// <summary>
    /// 產生檔案直接下載位置
    /// </summary>
    public class PostExternalDirectLinkRequest
    {
        /// <summary>
        /// 強制指定儲存個體群組 ID
        /// </summary>
        /// <example></example>
        public Guid? StorageGroupId { get; set; }
        /// <summary>
        /// 過期分鐘
        /// </summary>
        /// <example>60</example>
        public int ExpireAfterMinutes { get; set; }
        /// <summary>
        /// 存取次數限制
        /// </summary>
        /// <example>10</example>
        public int AccessTimesLimit { get; set; }
    }
}
