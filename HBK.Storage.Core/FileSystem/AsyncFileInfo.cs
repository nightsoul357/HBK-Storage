using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem
{
    /// <summary>
    /// 儲存服務檔案資訊
    /// </summary>
    public abstract class AsyncFileInfo : IAsyncFileInfo
    {
        /// <summary>
        /// 取得檔案是否存在
        /// </summary>
        public virtual bool Exists { get; protected set; }
        /// <summary>
        /// 取得檔案長度
        /// </summary>
        public virtual long Length { get; protected set; }
        /// <summary>
        /// 取得檔案路徑
        /// </summary>
        public virtual string PhysicalPath { get; protected set; }
        /// <summary>
        /// 取得檔案名稱
        /// </summary>
        public virtual string Name { get; protected set; }
        /// <summary>
        /// 取得檔案修改時間
        /// </summary>
        public virtual DateTimeOffset LastModified { get; protected set; }
        /// <summary>
        /// 取得是否為目錄
        /// </summary>
        public virtual bool IsDirectory { get; protected set; }

        /// <summary>
        /// 取得檔案資料流
        /// </summary>
        /// <returns></returns>
        public virtual Stream CreateReadStream()
        {
            return this.CreateReadStreamAsync().Result;
        }

        /// <summary>
        /// 取得檔案資料流
        /// </summary>
        public abstract Task<Stream> CreateReadStreamAsync();
    }
}
