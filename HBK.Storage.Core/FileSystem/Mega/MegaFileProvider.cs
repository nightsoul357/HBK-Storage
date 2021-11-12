using CG.Web.MegaApiClient;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.Mega
{
    /// <summary>
    /// Mega 儲存服務提供者
    /// </summary>
    public class MegaFileProvider : AsyncFileProvider
    {
        private readonly MegaApiClient _megaApiClient;
        /// <summary>
        /// 取得父系資料夾 ID
        /// </summary>
        public string ParentId { get; }
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="name">提供者名稱</param>
        /// <param name="username">帳號</param>
        /// <param name="password">密碼</param>
        /// <param name="parentId"></param>
        public MegaFileProvider(string name, string username, string password, string parentId)
            : base(name)
        {
            _megaApiClient = new MegaApiClient();
            _megaApiClient.Login(username, password);
            this.ParentId = parentId;
        }

        /// <inheritdoc/>
        public async override Task DeleteAsync(string subpath)
        {
            await _megaApiClient.DeleteAsync(await this.FindNodeByIdAsync(subpath));
        }

        /// <inheritdoc/>
        public override Task<IDirectoryContents> GetDirectoryContentsAsync(string subpath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async override Task<IAsyncFileInfo> GetFileInfoAsync(string subpath)
        {
            var node = await this.FindNodeByIdAsync(subpath);
            return new MegaFileInfo(_megaApiClient, node);
        }

        /// <inheritdoc/>
        public async override Task<bool> IsFileExistAsync(string subpath)
        {
            return !((await this.FindNodeByIdAsync(subpath)) == null);
        }

        /// <inheritdoc/>
        public async override Task<IAsyncFileInfo> PutAsync(string subpath, Stream fileStream)
        {
            var parent = await this.FindNodeByIdAsync(this.ParentId);
            using (MemoryStream memoryStream = new MemoryStream()) // Mega 必須事先知道上傳檔案的大小才能進行上傳
            {
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var node = await _megaApiClient.UploadAsync(memoryStream, subpath, parent, modificationDate: DateTime.Now);
                return new MegaFileInfo(_megaApiClient, node);
            }
        }

        /// <inheritdoc/>
        public override IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }

        private async Task<INode> FindNodeByIdAsync(string id)
        {
            return (await _megaApiClient.GetNodesAsync()).FirstOrDefault(x => x.Id == id);
        }
    }
}
