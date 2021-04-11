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
    /// <summary>
    /// 本地儲存服務提供者
    /// </summary>
    public class LocalFileProvider : AsyncFileProvider
    {
        /// <summary>
        /// 取得目錄
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// 取得緩衝區大小
        /// </summary>
        public int BufferSize { get; } = 5 * 1024 * 1024;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="name"></param>
        /// <param name="directory"></param>
        public LocalFileProvider(string name, string directory)
            : base(name)
        {
            this.Directory = directory;
        }

        /// <inheritdoc/>
        public override Task DeleteAsync(string subpath)
        {
            File.Delete(Path.Combine(this.Directory, subpath));
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task<IDirectoryContents> GetDirectoryContentsAsync(string subpath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<IAsyncFileInfo> GetFileInfoAsync(string subpath)
        {
            return Task<IAsyncFileInfo>.Run(() =>
            {
                return (IAsyncFileInfo)new LocalFileInfo(subpath, new FileInfo(Path.Combine(this.Directory, subpath)));
            });
        }

        /// <inheritdoc/>
        public override Task<bool> IsFileExistAsync(string subpath)
        {
            return Task<bool>.Run(() =>
            {
                return File.Exists(Path.Combine(this.Directory, subpath));
            });
        }

        /// <inheritdoc/>
        public override async Task<IAsyncFileInfo> PutAsync(string subpath, Stream fileStream)
        {
            if (await this.IsFileExistAsync(subpath))
            {
                throw new ArgumentException($"{subpath} exist already.");
            }

            byte[] buffer = new byte[this.BufferSize];
            var writeStream = File.Create(Path.Combine(this.Directory, subpath));
            int read;
            while ((read = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                writeStream.Write(buffer, 0, read);
            }
            writeStream.Flush();
            fileStream.Close();
            writeStream.Close();

            return await this.GetFileInfoAsync(subpath);
        }

        /// <inheritdoc/>
        public override IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
