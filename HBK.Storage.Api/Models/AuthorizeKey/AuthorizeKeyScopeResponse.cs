using HBK.Storage.Adapter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.AuthorizeKey
{
    /// <summary>
    /// 驗證金鑰使用範圍回應內容
    /// </summary>
    public class AuthorizeKeyScopeResponse
    {
        /// <summary>
        /// 儲存服務 ID
        /// </summary>
        public Guid StorageProviderId { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTimeOffset CreateDateTime { get; set; }
        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTimeOffset? UpdateDateTime { get; set; }
        /// <summary>
        /// 允許的操作類型
        /// </summary>
        public AuthorizeKeyScopeOperationTypeEnum AllowOperationType { get; set; }
    }
}
