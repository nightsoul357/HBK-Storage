using HBK.Storage.Api.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.StorageGroup
{
    /// <summary>
    /// 清除規則請求內容
    /// </summary>
    public class ClearPolicyRequest
    {
        /// <summary>
        /// 規則
        /// </summary>
        /// <example>ValidFileEntityStorageCount > 2</example>
        [ClearRule]
        public string Rule { get; set; }
    }
}
