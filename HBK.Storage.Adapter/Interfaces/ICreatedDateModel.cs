using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Interfaces
{
    /// <summary>
    /// 支援建立時間模型介面
    /// </summary>
    public interface ICreatedDateModel
    {
        /// <summary>
        /// 取得或設定建立時間
        /// </summary>
        DateTimeOffset CreateDateTime { get; internal set; }
    }
}
