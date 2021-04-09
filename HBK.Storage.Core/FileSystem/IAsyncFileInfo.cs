using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem
{
    /// <summary>
    /// 儲存服務檔案資訊介面
    /// </summary>
    public interface IAsyncFileInfo : IFileInfo
    {
        /// <summary>
        /// 取得檔案資料流
        /// </summary>
        /// <returns></returns>
        Task<Stream> CreateReadStreamAsync();
    }
}
