using HBK.Storage.Adapter.Enums;
using HBK.Storage.Api.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.AuthorizeKey
{
    /// <summary>
    /// 新增驗證金鑰請求內容
    /// </summary>
    public class AddAuthorizeKeyRequest
    {
        /// <summary>
        /// 名稱
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        [StatusEnum(typeof(AuthorizeKeyTypeEnum))]
        public AuthorizeKeyTypeEnum Type { get; set; }
        /// <summary>
        /// 驗證金鑰使用範圍清單
        /// </summary>
        public List<AddAuthorizeKeyScopeRequest> AddAuthorizeKeyScopeRequests { get; set; }
    }
}
