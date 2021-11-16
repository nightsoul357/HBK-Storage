using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.WebDAV
{
    /// <summary>
    /// WebDAV 的檔案資訊
    /// </summary>
    public class WebDAVFileInfo : AsyncFileInfo
    {
        private readonly WebDAVSerivce _webDAVSerivce;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="webDAVSerivce">WebDAV 存取邏輯</param>
        /// <param name="key">檔案主鍵</param>
        /// <param name="length">檔案大小</param>
        /// <param name="lastModified">最後修訂時間</param>
        /// <param name="physicalPath">檔案實際路徑</param>
        internal WebDAVFileInfo(WebDAVSerivce webDAVSerivce, string key, long length, DateTimeOffset lastModified, string physicalPath)
        {
            _webDAVSerivce = webDAVSerivce;
            this.Name = key;
            this.Length = length;
            this.LastModified = lastModified;
            this.PhysicalPath = physicalPath;
            this.Exists = !(length == 0);
            this.IsDirectory = false;
        }

        /// <inheritdoc/>
        public override Task<Stream> CreateReadStreamAsync()
        {
            return Task.FromResult((Stream)new WebDAVFileStream(_webDAVSerivce, this.Name, this.Length));
        }
    }
}
