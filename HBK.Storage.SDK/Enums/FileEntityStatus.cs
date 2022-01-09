using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// [Flags] 檔案實體狀態列舉
    /// <br/>
    /// <br/>* **uploading** - 上傳中
    /// <br/>* **processing** - 處理中
    /// <br/>
    /// </summary>
    public enum FileEntityStatus
    {
        [System.Runtime.Serialization.EnumMember(Value = @"uploading")]
        Uploading = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"processing")]
        Processing = 1,
    }
}
