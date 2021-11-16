using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.WebDAV
{
    /// <summary>
    /// Web DAV 的檔案提供者
    /// </summary>
    public class WebDAVFileProvider : AsyncFileProvider, IDisposable
    {
        private readonly WebDAVSerivce _webDAVSerivce;
        private readonly bool _isSupportPartialUpload;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isSupportPartialUpload"></param>
        public WebDAVFileProvider(string name, string url, string username, string password, bool isSupportPartialUpload)
            : base(name)
        {
            _isSupportPartialUpload = isSupportPartialUpload;
            _webDAVSerivce = new WebDAVSerivce(new WebDAVOption()
            {
                Url = url,
                Username = username,
                Password = password
            });
        }

        /// <inheritdoc/>
        public override Task DeleteAsync(string subpath)
        {
            return _webDAVSerivce.DeleteAsync(subpath);
        }



        /// <inheritdoc/>
        public override Task<IDirectoryContents> GetDirectoryContentsAsync(string subpath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async override Task<IAsyncFileInfo> GetFileInfoAsync(string subpath)
        {
            var fileInfo = await _webDAVSerivce.GetFileInfoAsync(subpath);
            return fileInfo;
        }

        /// <inheritdoc/>
        public override Task<bool> IsFileExistAsync(string subpath)
        {
            return _webDAVSerivce.IsFileExistAsync(subpath);
        }

        /// <inheritdoc/>
        public async override Task<IAsyncFileInfo> PutAsync(string subpath, Stream fileStream)
        {
            if (_isSupportPartialUpload)
            {
                await this.PutPartialAsync(subpath, fileStream);
                return await this.GetFileInfoAsync(subpath);
            }
            else
            {

                if (fileStream.Length == 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await fileStream.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        fileStream.Close();

                        var fileInfo = await _webDAVSerivce.PutAsync(subpath, memoryStream);
                        return fileInfo;
                    }
                }
                else
                {
                    var fileInfo = await _webDAVSerivce.PutAsync(subpath, fileStream);
                    fileStream.Close();
                    return fileInfo;
                }
            }
        }

        /// <inheritdoc/>
        public override IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _webDAVSerivce.Dispose();
        }

        private async Task PutPartialAsync(string subpath, Stream fileStream)
        {
            byte[] buffer = new byte[65536];
            int read = 0;
            int position = 0;

            while ((read = fileStream.Read(buffer, read, buffer.Length)) != 0)
            {
                using (var momoeryStream = new MemoryStream(buffer, 0, read))
                {
                    await _webDAVSerivce.PutPartialAsync(subpath, momoeryStream, position, position + read);
                }
            }
        }
    }
}
