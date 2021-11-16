using HBK.Storage.Adapter.Enums;
using HBK.Storage.Api.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.AuthorizeKey
{
    /// <summary>
    /// 新增驗證金鑰使用範圍請求內容
    /// </summary>
    public class AddAuthorizeKeyScopeRequest
    {
        /// <summary>
        /// 儲存服務 ID
        /// </summary>
        [ExistInDatabase(typeof(Adapter.Storages.StorageProvider))]
        public Guid StorageProviderId { get; set; }
        /// <summary>
        /// 允許的操作類型
        /// </summary>
        [StatusEnum(typeof(AuthorizeKeyScopeOperationTypeEnum))]
        public AuthorizeKeyScopeOperationTypeEnum AllowOperationType { get; set; }

    }
}
