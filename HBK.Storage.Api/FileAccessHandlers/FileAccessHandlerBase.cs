using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 存取檔案處理器基底
    /// </summary>
    public abstract class FileAccessHandlerBase
    {
        /// <summary>
        /// 處理檔案
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public abstract Task<FileAccessTaskModel> ProcessAsync(FileAccessTaskModel taskModel);
        /// <summary>
        /// 取得處理器名稱
        /// </summary>
        public abstract string Name { get; }
    }
}
