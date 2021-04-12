using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Enums
{
    /// <summary>
    /// 存取檔案權杖類型
    /// </summary>
    public enum FileAccessTokenTypeEnum : int
    {
        /// <summary>
        /// 一般
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 無限制的一般
        /// </summary>
        NormalNoLimit = 2,
        /// <summary>
        /// 允許指定的 Tag
        /// </summary>
        AllowTag = 3,
        /// <summary>
        /// 無限制的允許指定的 Tag
        /// </summary>
        AllowTagNoLimit = 4
    }
}
