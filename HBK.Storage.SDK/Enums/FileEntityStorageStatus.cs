using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// [Flags] 檔案位於儲存個體上的橋接資訊狀態列舉
    /// <br/>
    /// <br/>* **syncing** - 正在同步
    /// <br/>* **sync_fail** - 同步失敗
    /// <br/>* **disable** - 停用
    /// <br/>
    /// </summary>
    public enum FileEntityStorageStatus
    {
        [System.Runtime.Serialization.EnumMember(Value = @"syncing")]
        Syncing = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"sync_fail")]
        SyncFail = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"disable")]
        Disable = 2,
    }
}
