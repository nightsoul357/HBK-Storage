using HBK.Storage.Api.FileAccessHandlers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 檔案存取處理器介面
    /// </summary>
    public interface IFileAccessHandler
    {
        /// <summary>
        /// 處理檔案
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        Task<FileAccessTaskModel> ProcessAsync(FileAccessTaskModel taskModel);
    }
}
