using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Interfaces
{
    /// <summary>
    /// 支援建立時間及更新時間的模型介面
    /// </summary>
    public interface ITimeStampModel : ICreatedDateModel
    {
        /// <summary>
        /// 取得或設定更新時間
        /// </summary>
        DateTimeOffset? UpdateDateTime { get; internal set; }
    }
}
