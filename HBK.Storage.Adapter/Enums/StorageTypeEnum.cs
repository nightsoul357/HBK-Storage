using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 儲存個體類型列舉
    /// </summary>
    public enum StorageTypeEnum : int
    {
        /// <summary>
        /// FTP 協議所定義的儲存個體
        /// </summary>
        FTP = 1,
        /// <summary>
        /// Amazon S3 的儲存個體
        /// </summary>
        AmazonS3 = 2,
        /// <summary>
        /// 本地儲存空間的儲存個體
        /// </summary>
        Local = 3
    }
}
