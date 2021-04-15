using HBK.Storage.Adapter.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.StorageProvider
{
    /// <summary>
    /// 更新儲存服務請求內容
    /// </summary>
    public class StorageProviderUpdateRequest
    {
        /// <summary>
        /// 名稱
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public StorageProviderStatusEnum[] Status { get; set; }
    }
}
