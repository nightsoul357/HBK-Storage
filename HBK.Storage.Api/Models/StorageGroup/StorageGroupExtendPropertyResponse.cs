using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.StorageGroup
{
    /// <summary>
    /// 儲存群組額外資訊回應內容
    /// </summary>
    public class StorageGroupExtendPropertyResponse : StorageGroupResponse
    {
        /// <summary>
        /// 大小限制(Bytes)
        /// </summary>
        public long? SizeLimit { get; set; }
        /// <summary>
        /// 已使用大小(Bytes)
        /// </summary>
        public long? UsedSize { get; set; }
    }
}
