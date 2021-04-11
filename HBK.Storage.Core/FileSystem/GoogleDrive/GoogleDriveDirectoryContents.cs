using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.GoogleDrive
{
    /// <summary>
    /// Google Drive 目錄內容
    /// </summary>
    public class GoogleDriveDirectoryContents : IDirectoryContents
    {
        private readonly List<GoogleDriveFileInfo> _files;
        private readonly bool _isExist;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="isExist">資料夾是否存在</param>
        /// <param name="files">檔案清單</param>

        public GoogleDriveDirectoryContents(bool isExist, IEnumerable<GoogleDriveFileInfo> files)
        {
            _isExist = isExist;
            _files = files.ToList();
        }

        /// <summary>
        /// 取得檔案室否存在
        /// </summary>
        public bool Exists => _isExist;

        /// <summary>
        /// 取得列舉器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFileInfo> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
