using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.Local
{
    /// <summary>
    /// 本地的檔案資訊
    /// </summary>
    public class LocalFileInfo : AsyncFileInfo
    {
        private FileInfo _fileInfo;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="key">檔案主鍵</param>
        /// <param name="fileInfo">檔案資訊</param>
        public LocalFileInfo(string key, FileInfo fileInfo)
        {
            this.Name = key;
            this.Length = fileInfo.Length;
            this.LastModified = fileInfo.LastWriteTime;
            this.PhysicalPath = fileInfo.FullName;
            this.Exists = fileInfo.Exists;
            this.IsDirectory = false;
            _fileInfo = fileInfo;
        }

        /// <inheritdoc/>
        public override Task<Stream> CreateReadStreamAsync()
        {
            if (!this.Exists)
            {
                throw new FileNotFoundException();
            }

            return Task<Stream>.Run(() =>
            {
                return (Stream)File.Open(_fileInfo.FullName, FileMode.Open);
            });
        }
    }
}
