using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// 存取檔案權杖類型
    /// <br/>
    /// <br/>* **normal** - 一般
    /// <br/>* **normal_no_limit** - 無限制的一般
    /// <br/>* **allow_tag** - 允許指定的 Tag
    /// <br/>* **allow_tag_no_limit** - 無限制的允許指定的 Tag
    /// <br/>
    /// </summary>
    public enum FileAccessTokenType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"normal")]
        Normal = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"normal_no_limit")]
        NormalNoLimit = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"allow_tag")]
        AllowTag = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"allow_tag_no_limit")]
        AllowTagNoLimit = 3,
    }
}
