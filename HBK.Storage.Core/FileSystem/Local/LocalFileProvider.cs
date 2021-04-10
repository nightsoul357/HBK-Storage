using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.Local
{
    public class LocalFileProvider : AsyncFileProvider
    {
        private readonly string _directory;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="name"></param>
        /// <param name="directory"></param>
        public LocalFileProvider(string name,string directory) 
            : base(name)
        {
            _directory = directory;
        }
        public override Task DeleteAsync(string subpath)
        {
            throw new NotImplementedException();
        }

        public override Task<IDirectoryContents> GetDirectoryContentsAsync(string subpath)
        {
            throw new NotImplementedException();
        }

        public override Task<IAsyncFileInfo> GetFileInfoAsync(string subpath)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> IsFileExistAsync(string subpath)
        {
            throw new NotImplementedException();
        }

        public override Task<IAsyncFileInfo> PutAsync(string subpath, Stream fileStream)
        {
            throw new NotImplementedException();
        }

        public override IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
