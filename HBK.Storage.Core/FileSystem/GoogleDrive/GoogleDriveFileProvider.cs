using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.GoogleDrive
{
    /// <summary>
    /// Google Drive 儲存服務提供者
    /// </summary>
    public class GoogleDriveFileProvider : AsyncFileProvider
    {
        private DriveService _driveService;
        /// <summary>
        /// 取得緩衝區大小
        /// </summary>
        public int BufferSize { get; } = 5 * 1024 * 1024;
        /// <summary>
        /// 取得父資料夾 ID
        /// </summary>

        public string Parent { get; set; }
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="name">名稱</param>
        /// <param name="parent">父資料夾 ID</param>
        /// <param name="clientId">存取金鑰</param>
        /// <param name="clientSecret">存取密鑰</param>
        /// <param name="user">使用者名稱</param>
        /// <param name="dataStore">Token 存取方式</param>
        public GoogleDriveFileProvider(string name, string parent, string clientId, string clientSecret, string user, IDataStore dataStore)
            : base(name)
        {
            this.Parent = parent;
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                   new ClientSecrets()
                   {
                       ClientId = clientId,
                       ClientSecret = clientSecret
                   }, new string[] { DriveService.Scope.Drive },
                   user,
                   CancellationToken.None,
                   dataStore).Result;

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = name,
            });
        }

        /// <inheritdoc/>
        public override async Task DeleteAsync(string subpath)
        {
            _ = await _driveService.Files.Delete(subpath).ExecuteAsync();
        }

        /// <inheritdoc/>
        public override Task<IDirectoryContents> GetDirectoryContentsAsync(string subpath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override async Task<IAsyncFileInfo> GetFileInfoAsync(string subpath)
        {
            var request = _driveService.Files.Get(subpath);
            request.Fields = "*";
            var file = await request.ExecuteAsync();
            return new GoogleDriveFileInfo(_driveService, file);
        }

        /// <inheritdoc/>
        public override async Task<bool> IsFileExistAsync(string subpath)
        {
            var request = _driveService.Files.Get(subpath);
            request.Fields = "*";
            var file = await request.ExecuteAsync();
            return !(file == null);
        }

        /// <inheritdoc/>
        public override async Task<IAsyncFileInfo> PutAsync(string subpath, Stream fileStream)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File() { Name = subpath };
            if (!String.IsNullOrWhiteSpace(this.Parent))
            {
                fileMetadata.Parents = new List<string>() { this.Parent };
            }

            FilesResource.CreateMediaUpload request = _driveService.Files.Create(fileMetadata, fileStream, "application/unknown");
            await request.UploadAsync();

            return await this.GetFileInfoAsync(request.ResponseBody.Id);
        }

        /// <inheritdoc/>
        public override IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
