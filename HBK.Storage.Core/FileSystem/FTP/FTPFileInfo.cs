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
    /// FTP 的檔案資訊
    /// </summary>
    public class FTPFileInfo : AsyncFileInfo
    {
        private FTPOption _ftpOption;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="ftpOption">FTP 連接選項</param>
        /// <param name="key">檔案主鍵</param>
        /// <param name="length">檔案大小</param>
        /// <param name="lastModified">最後修訂時間</param>
        /// <param name="physicalPath">檔案實際路徑</param>
        internal FTPFileInfo(FTPOption ftpOption, string key, long length, DateTimeOffset lastModified, string physicalPath)
        {
            _ftpOption = ftpOption;
            this.Name = key;
            this.Length = length;
            this.LastModified = lastModified;
            this.PhysicalPath = physicalPath;
            this.Exists = !(length == 0);
            this.IsDirectory = false;
        }

        /// <summary>
        /// 取得檔案資料
        /// </summary>
        /// <returns></returns>
        public override Task<Stream> CreateReadStreamAsync()
        {
            if (!this.Exists)
            {
                throw new FileNotFoundException();
            }

            string requestUrl = _ftpOption.Url + "/" + this.Name;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUrl);
            request.Credentials = _ftpOption.Credentials;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            return Task.FromResult(responseStream);
        }
    }
}
