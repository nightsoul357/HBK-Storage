using HBK.Storage.Adapter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.AuthorizeKey
{
    /// <summary>
    /// 驗證金鑰回應內容
    /// </summary>
    public class AuthorizeKeyResponse
    {
        /// <summary>
        /// 金鑰 ID
        /// </summary>
        public Guid AuthorizeKeyId { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 金鑰值
        /// </summary>
        public string KeyValue { get; set; }
        /// <summary>
        /// 取得建立時間
        /// </summary>
        public DateTimeOffset CreateDateTime { get; set; }
        /// <summary>
        /// 取得最後更新時間
        /// </summary>
        public DateTimeOffset? UpdateDateTime { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public AuthorizeKeyTypeEnum Type { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public AuthorizeKeyStatusEnum[] Status { get; set; }
        /// <summary>
        /// 驗證用的金鑰資訊清單
        /// </summary>
        public List<AuthorizeKeyScopeResponse> AuthorizeKeyScopeResponses { get; set; }
    }
}
