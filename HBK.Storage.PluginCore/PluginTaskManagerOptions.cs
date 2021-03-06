using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.PluginCore
{
    /// <summary>
    /// 插件任務管理器選項
    /// </summary>
    public class PluginTaskManagerOptions
    {
        /// <summary>
        /// 取得或設定識別名稱
        /// </summary>
        public string Identity { get; set; }
        /// <summary>
        /// 取得或設定同時工作數量上限
        /// </summary>
        public int TaskLimit { get; set; } = 5;
        /// <summary>
        /// 取得或設定是否在所有儲存服務上執行
        /// </summary>
        public bool IsExecutOnAllStoragProvider { get; set; } = true;
        /// <summary>
        /// 取得或設定作用的儲存服務 ID 清單
        /// </summary>
        public List<Guid> StorageProviderIds { get; set; }
        /// <summary>
        /// 取得或設定是否作用於 Local Storage
        /// </summary>
        public bool IsExecuteOnLocalStorage { get; set; } = false;
        /// <summary>
        /// 取得或設定檔案實體流水號除數
        /// </summary>
        public int FileEntityNoDivisor { get; set; } = 1;
        /// <summary>
        /// 取得或設定檔案流水號餘數
        /// </summary>
        public int FileEntityNoRemainder { get; set; } = 0;
        /// <summary>
        /// 取得或設定單次取得同步任務數量上限
        /// </summary>
        public int FetchLimit { get; set; } = 100;
        /// <summary>
        /// 取得或設定是否僅作用在根檔案(非從屬檔案)
        /// </summary>
        public bool JustExecuteOnRootFileEntity { get; set; } = true;
        /// <summary>
        /// 取得或設定 MimeType 範本
        /// </summary>
        public string MimeTypePartten { get; set; } = "%";
        /// <summary>
        /// 取得或設定取資料方式
        /// </summary>
        public FetchModeEnum FetchMode { get; set; } = FetchModeEnum.WithoutTag;
        /// <summary>
        /// 取得或設定識別標籤
        /// </summary>
        public string IdentityTag { get; set; }
        /// <summary>
        /// 取得或設定例外標籤
        /// </summary>
        public string ExceptionTag { get; set; }
        /// <summary>
        /// 取得或設定間隔時間
        /// </summary>
        public int Interval { get; set; } = 1000;
        /// <summary>
        /// 取得或設定是否自動重試
        /// </summary>
        public bool IsAutoRetry { get; set; } = true;
        /// <summary>
        /// 取得或設定自動重試間隔
        /// </summary>
        public int AutoRetryInterval { get; set; } = 10000;
        /// <summary>
        /// 取得或設定失敗次數判定
        /// </summary>
        public int FailTimes { get; set; } = 3;
    }
}