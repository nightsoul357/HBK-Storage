using HBK.Storage.Adapter.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Sync.Model
{
    /// <summary>
    /// 同步任務資料
    /// </summary>
    public class SyncTaskModel
    {
        /// <summary>
        /// 取得或設定檔案實體
        /// </summary>
        public FileEntity FileEntity { get; set; }
        /// <summary>
        /// 取得或設定檔案來源之檔案群組
        /// </summary>
        public StorageGroup FromStorageGroup { get; set; }
        /// <summary>
        /// 取得或設定來源檔案位於儲存個體上的橋接資訊
        /// </summary>
        public FileEntityStorage FromFileEntityStorage { get; set; }
        /// <summary>
        /// 取得或設定檔案目的之檔案群組
        /// </summary>
        public StorageGroup DestinationStorageGroup { get; set; }
        /// <summary>
        /// 取得或設定目標檔案位於儲存個體上的橋接資訊
        /// </summary>
        public FileEntityStorage DestinationFileEntityStorage { get; set; }
        /// <summary>
        /// 取得或設定建立時間
        /// </summary>
        public DateTimeOffset CreateDateTime { get; set; }
        /// <summary>
        /// 取得或設定開始時間
        /// </summary>
        public DateTimeOffset StratDateTime { get; set; }
        /// <summary>
        /// 取得或設定結束時間
        /// </summary>
        public DateTimeOffset EndDateTime { get; set; }
    }
}
