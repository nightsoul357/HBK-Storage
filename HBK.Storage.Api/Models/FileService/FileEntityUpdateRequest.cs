using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileService
{
    /// <summary>
    /// 更新檔案實體請求內容
    /// </summary>
    public class FileEntityUpdateRequest
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        [Required]
        public string Filename { get; set; }
        /// <summary>
        /// 擴充資訊
        /// </summary>
        [Required]
        public string ExtendProperty { get; set; }
    }
}
