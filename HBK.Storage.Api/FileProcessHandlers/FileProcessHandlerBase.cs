using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileProcessHandlers
{
    /// <summary>
    /// 處理檔案處理器基底
    /// </summary>
    public abstract class FileProcessHandlerBase
    {
        /// <summary>
        /// 處理檔案
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public abstract Task<FileProcessTaskModel> ProcessAsync(FileProcessTaskModel taskModel);
        /// <summary>
        /// 取得處理器名稱
        /// </summary>
        public abstract string Name { get; }
    }
}
