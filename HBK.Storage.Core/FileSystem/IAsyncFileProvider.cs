using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace HBK.Storage.Core.FileSystem
{
    /// <summary>
    /// 儲存服務提供者介面
    /// </summary>
    public interface IAsyncFileProvider : IFileProvider
    {
        /// <summary>
        /// 取得提供者名稱
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        Task DeleteAsync(string subpath);

        /// <summary>
        /// 刪除多個檔案
        /// </summary>
        /// <param name="subpathes">檔案相對路徑集合</param>
        Task DeleteAsync(string[] subpathes);

        /// <summary>
        /// 新增檔案
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <param name="fileStream">檔案流</param>
        /// <returns></returns>
        Task<IAsyncFileInfo> PutAsync(string subpath, Stream fileStream);

        /// <summary>
        /// 指定的檔案是否存在
        /// </summary>
        /// <param name="subpath">檔案的相對路徑</param>
        /// <returns></returns>
        Task<bool> IsFileExistAsync(string subpath);

        /// <summary>
        /// 取得目錄下的檔案
        /// </summary>
        /// <param name="subpath">相對路徑</param>
        /// <returns></returns>
        Task<IDirectoryContents> GetDirectoryContentsAsync(string subpath);

        /// <summary>
        /// 取得檔案資訊
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <returns></returns>
        Task<IAsyncFileInfo> GetFileInfoAsync(string subpath);
    }
}
