using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 金鑰類型列舉
    /// </summary>
    public enum AuthorizeKeyTypeEnum : int
    {
        /// <summary>
        /// 最高權限的金鑰，可以存取所有服務
        /// </summary>
        Root = 1,
        /// <summary>
        /// 一般的金鑰，根據 Scope 的定義存取服務
        /// </summary>
        Normal = 2
    }
}
