using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileService
{
    /// <summary>
    /// 產生檔案存取權杖請求內容
    /// </summary>
    public class PostFileAccessTokenRequest
    {
        /// <summary>
        /// 強制指定檔案群組 ID
        /// </summary>
        /// <example></example>
        public Guid? StorageGroupId { get; set; }
        /// <summary>
        /// 檔案實體 ID
        /// </summary>
        public Guid? FileEntityId { get; set; }
        /// <summary>
        /// 允許 Tag 通過的格式
        /// </summary>
        public string AllowTagPattern { get; set; }
        /// <summary>
        /// 存取檔案權杖類型
        /// </summary>
        [StatusEnum(typeof(FileAccessTokenTypeEnum))]
        public FileAccessTokenTypeEnum FileAccessTokenType { get; set; }
        /// <summary>
        /// 過期分鐘
        /// </summary>
        /// <example>60</example>
        public int ExpireAfterMinutes { get; set; }
        /// <summary>
        /// 存取次數限制
        /// </summary>
        /// <example>10</example>
        public int? AccessTimesLimit { get; set; }
        /// <summary>
        /// 檔案處理器指示字串
        /// </summary>
        public string HandlerIndicate { get; set; }
    }
}
