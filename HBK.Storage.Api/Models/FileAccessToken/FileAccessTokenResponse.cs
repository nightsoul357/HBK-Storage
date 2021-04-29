using HBK.Storage.Adapter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileAccessToken
{
    /// <summary>
    /// 檔案存取權杖回應內容
    /// </summary>
    public class FileAccessTokenResponse
    {
        /// <summary>
        /// 檔案存取權杖 ID
        /// </summary>
        public Guid FileAccessTokenId { get; set; }
        /// <summary>
        /// 存取對象的儲存服務 ID
        /// </summary>
        public Guid StorageProviderId { get; set; }
        /// <summary>
        /// 強制指定的存取對象的儲存個體集合 ID
        /// </summary>
        public Guid? StorageGroupId { get; set; }
        /// <summary>
        /// 檔案實體 ID
        /// </summary>
        public Guid? FileEntityId { get; set; }
        /// <summary>
        /// 權杖
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 存取次數限制
        /// </summary>
        public int AccessTimesLimit { get; set; }
        /// <summary>
        /// 存取次數
        /// </summary>
        public int AccessTimes { get; set; }
        /// <summary>
        /// 過期時間
        /// </summary>
        public DateTimeOffset ExpireDateTime { get; set; }
        /// <summary>
        /// 取得建立時間
        /// </summary>
        public DateTimeOffset CreateDateTime { get; internal set; }
        /// <summary>
        /// 取得最後更新時間
        /// </summary>
        public DateTimeOffset? UpdateDateTime { get; internal set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public FileAccessTokenStatusEnum[] Status { get; set; }
    }
}