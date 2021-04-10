using Microsoft.Extensions.FileProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.Local
{
    /// <summary>
    /// 本地目錄內容
    /// </summary>
    public class LocalDirectoryContents : IDirectoryContents
    {
        private readonly List<LocalFileInfo> _files;
        private readonly bool _isExist;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="isExist">資料夾是否存在</param>
        /// <param name="files">檔案清單</param>

        public LocalDirectoryContents(bool isExist, IEnumerable<LocalFileInfo> files)
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
