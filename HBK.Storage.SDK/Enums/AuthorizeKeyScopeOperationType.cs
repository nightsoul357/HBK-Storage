using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// 金鑰使用範圍之操作類型
    /// <br/>
    /// <br/>* **insert** - 新增
    /// <br/>* **delete** - 刪除
    /// <br/>* **update** - 修改
    /// <br/>* **read** - 查詢
    /// <br/>
    /// </summary>
    public enum AuthorizeKeyScopeOperationType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"insert")]
        Insert = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"delete")]
        Delete = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"update")]
        Update = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"read")]
        Read = 3,
    }
}
