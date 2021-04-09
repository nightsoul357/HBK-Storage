using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Interfaces
{
    /// <summary>
    /// 支援軟刪除的模型介面
    /// </summary>
    public interface ISoftDeleteModel
    {
        /// <summary>
        /// 取得或設定建立刪除時間
        /// </summary>
        DateTimeOffset? DeleteDateTime { get; internal set; }
    }
}
