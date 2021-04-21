using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 金鑰使用範圍之操作類型
    /// </summary>
    public enum AuthorizeKeyScopeOperationTypeEnum : int
    {
        /// <summary>
        /// 新增
        /// </summary>
        Insert = 1,
        /// <summary>
        /// 刪除
        /// </summary>
        Delete = 2,
        /// <summary>
        /// 修改
        /// </summary>
        Update = 3,
        /// <summary>
        /// 查詢
        /// </summary>
        Read = 4
    }
}
