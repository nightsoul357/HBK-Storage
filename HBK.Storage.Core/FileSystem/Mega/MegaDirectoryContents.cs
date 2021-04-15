using Microsoft.Extensions.FileProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.Mega
{
    /// <summary>
    /// Mega 目錄內容
    /// </summary>
    public class MegaDirectoryContents : IDirectoryContents
    {
        private readonly List<MegaFileInfo> _files;
        private readonly bool _isExist;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="isExist">資料夾是否存在</param>
        /// <param name="files">檔案清單</param>

        public MegaDirectoryContents(bool isExist, IEnumerable<MegaFileInfo> files)
        {
            _isExist = isExist;
            _files = files.ToList();
        }
        /// <summary>
        /// 取得檔案室否存在
        /// </summary>
        public bool Exists => _isExist;
        
        /// <inheritdoc/>
        public IEnumerator<IFileInfo> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
