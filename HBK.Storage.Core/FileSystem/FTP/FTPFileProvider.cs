using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.FTP
{
    /// <summary>
    /// FTP 儲存服務提供者
    /// </summary>
    public class FTPFileProvider : AsyncFileProvider
    {
        private readonly FTPOption _ftpOption;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="name">提供者名稱</param>
        /// <param name="url">連接位置</param>
        /// <param name="username">使用者名稱</param>
        /// <param name="password">密碼</param>
        public FTPFileProvider(string name, string url, string username, string password)
            : base(name)
        {
            _ftpOption = new FTPOption()
            {
                Url = url,
                Credentials = new NetworkCredential(username, password)
            };
        }

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="name">提供者名稱</param>
        /// <param name="ftpOption">FTP 連接選項</param>
        public FTPFileProvider(string name, FTPOption ftpOption)
             : base(name)
        {
            _ftpOption = ftpOption;
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        public async override Task DeleteAsync(string subpath)
        {
            var request = this.BuildFTPWebRequest(subpath);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            using FtpWebResponse response = (FtpWebResponse)(await request.GetResponseAsync());
            var result = response.StatusDescription;
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="subpathes">檔案相對路徑集合</param>
        public override async Task DeleteAsync(string[] subpathes)
        {
            foreach (var subPath in subpathes)
            {
                await this.DeleteAsync(subPath);
            }
        }

        /// <summary>
        /// 取得目錄下的檔案
        /// </summary>
        /// <param name="subpath">相對路徑</param>
        /// <returns></returns>
        public override Task<IDirectoryContents> GetDirectoryContentsAsync(string subpath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取得檔案資訊
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <returns></returns>
        public async override Task<IAsyncFileInfo> GetFileInfoAsync(string subpath)
        {
            var requestForSize = this.BuildFTPWebRequest(subpath);
            requestForSize.Method = WebRequestMethods.Ftp.GetFileSize;
            using FtpWebResponse responseForSize = (FtpWebResponse)(await requestForSize.GetResponseAsync());
            long size = responseForSize.ContentLength;

            var requestForLastModified = this.BuildFTPWebRequest(subpath);
            requestForLastModified.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            FtpWebResponse responseForLastModified = (FtpWebResponse)(await requestForLastModified.GetResponseAsync());
            DateTimeOffset lastModified = responseForLastModified.LastModified;

            return new FTPFileInfo(_ftpOption, subpath, size, lastModified, this.BuildPhysicalPath(subpath));
        }
        /// <summary>
        /// 新增檔案
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <param name="fileStream">檔案流</param>
        /// <returns></returns>
        public override async Task<IAsyncFileInfo> PutAsync(string subpath, Stream fileStream)
        {
            var request = this.BuildFTPWebRequest(subpath);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            Stream writeStream = await request.GetRequestStreamAsync();
            byte[] buffer = new byte[_ftpOption.UploadBufferSize];
            int read;
            while ((read = await fileStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                writeStream.Write(buffer, 0, read);
            }
            writeStream.Flush();
            writeStream.Close();
            fileStream.Close();

            return await this.GetFileInfoAsync(subpath);
        }

        /// <summary>
        /// 指定的檔案是否存在
        /// </summary>
        /// <param name="subpath">檔案的相對路徑</param>
        /// <returns></returns>
        public async override Task<bool> IsFileExistAsync(string subpath)
        {
            var request = this.BuildFTPWebRequest(subpath);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)(await request.GetResponseAsync());
                return true;
            }
            catch (WebException ex) when (((FtpWebResponse)ex.Response).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                return false;
            }
        }

        /// <summary>
        /// 監視檔案變更
        /// </summary>
        /// <param name="filter">篩選器</param>
        /// <returns></returns>
        public override IChangeToken Watch(string filter)
        {
            throw new NotSupportedException();
        }
        private FtpWebRequest BuildFTPWebRequest(string subPath)
        {
            string requestUrl = this.BuildPhysicalPath(subPath);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUrl);
            request.Credentials = _ftpOption.Credentials;
            return request;
        }
        private string BuildPhysicalPath(string subPath)
        {
            return _ftpOption.Url + "/" + subPath;
        }
    }
}
