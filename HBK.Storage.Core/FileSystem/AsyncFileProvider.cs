using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace HBK.Storage.Core.FileSystem
{
    /// <summary>
    /// 儲存服務提供者
    /// </summary>
    public abstract class AsyncFileProvider : IAsyncFileProvider
    {
        /// <summary>
        /// 取得提供者的名字
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="name">提供者名稱</param>
        public AsyncFileProvider(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        public abstract Task DeleteAsync(string subpath);

        /// <summary>
        /// 新增檔案
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <param name="fileStream">檔案流</param>
        /// <returns></returns>
        public abstract Task<IAsyncFileInfo> PutAsync(string subpath, Stream fileStream);

        /// <summary>
        /// 指定的檔案是否存在
        /// </summary>
        /// <param name="subpath">檔案的相對路徑</param>
        /// <returns></returns>
        public abstract Task<bool> IsFileExistAsync(string subpath);

        /// <summary>
        /// 取得目錄下的檔案資訊
        /// </summary>
        /// <param name="subpath">相對路徑</param>
        /// <returns></returns>
        public abstract Task<IDirectoryContents> GetDirectoryContentsAsync(string subpath);

        /// <summary>
        /// 取得檔案資訊
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <returns></returns>
        public abstract Task<IAsyncFileInfo> GetFileInfoAsync(string subpath);

        /// <summary>
        /// 監看檔案是否發生變更
        /// </summary>
        /// <param name="filter">檔案名稱過濾器</param>
        /// <returns></returns>

        public abstract IChangeToken Watch(string filter);

        /// <summary>
        /// 取得檔案資訊
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <returns></returns>
        public IFileInfo GetFileInfo(string subpath)
        {
            return this.GetFileInfoAsync(subpath).Result;
        }

        /// <summary>
        /// 取得目錄下的檔案資訊
        /// </summary>
        /// <param name="subpath">相對路徑</param>
        /// <returns></returns>
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return this.GetDirectoryContentsAsync(subpath).Result;
        }

        /// <summary>
        /// 刪除多個檔案
        /// </summary>
        /// <param name="subpathes">檔案相對路徑集合</param>
        public virtual Task DeleteAsync(string[] subpathes)
        {
            return Task.WhenAll(subpathes.Select(subpath => this.DeleteAsync(subpath)));
        }
    }
}
