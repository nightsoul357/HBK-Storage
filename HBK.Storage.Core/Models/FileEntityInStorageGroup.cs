using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Models
{
    /// <summary>
    /// 檔案實體位於檔案檔案儲存群組的資訊
    /// </summary>
    public class FileEntityInStorageGroup
    {
        /// <summary>
        /// 取得或設定檔案實體
        /// </summary>
        public FileEntity FileEntity { get; set; }
        /// <summary>
        /// 取得或設定檔案位於儲存個體上的橋接資訊
        /// </summary>
        public FileEntityStorage FileEntityStorage { get; set; }
        /// <summary>
        /// 取得或設定該檔案有效的檔案位於儲存個體上的橋接資訊數量
        /// </summary>
        public int ValidFileEntityStorageCount { get; set; }
    }
}
