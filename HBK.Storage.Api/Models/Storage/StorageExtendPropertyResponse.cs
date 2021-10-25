using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.Storage
{
    /// <summary>
    /// 儲存個體擴充資訊回應內容
    /// </summary>
    public class StorageExtendPropertyResponse : StorageResponse
    {
        /// <summary>
        /// 已使用大小(Bytes)
        /// </summary>
        public long? UsedSize { get; set; }
    }
}
