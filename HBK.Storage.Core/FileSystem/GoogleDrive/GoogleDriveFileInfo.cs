using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.GoogleDrive
{
    /// <summary>
    /// Google Drive 的檔案資訊
    /// </summary>
    public class GoogleDriveFileInfo : AsyncFileInfo
    {
        private Google.Apis.Drive.v3.Data.File _file;
        private DriveService _driveService;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="driveService">Google Drive 服務</param>
        /// <param name="file">Google Drive 檔案</param>
        public GoogleDriveFileInfo(DriveService driveService, Google.Apis.Drive.v3.Data.File file)
        {
            this.Name = file.Id;
            this.Length = file.Size.Value;
            this.LastModified = file.ModifiedTime.Value;
            this.PhysicalPath = file.Name;
            this.Exists = file.Size.HasValue;
            this.IsDirectory = file.MimeType == "application/vnd.google-apps.folder";
            _file = file;
            _driveService = driveService;
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
                return (Stream)new GoogleDriveFileStreamV2(_driveService, _file);
            });
        }
    }
}
