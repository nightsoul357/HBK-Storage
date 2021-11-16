using HBK.Storage.Api.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.StorageGroup
{
    /// <summary>
    /// 同步規則請求內容
    /// </summary>
    public class SyncPolicyRequest
    {
        /// <summary>
        /// 規則
        /// </summary>
        /// <example>Name.Contains(\"test\")</example>
        [SyncRule]
        public string Rule { get; set; }
    }
}
