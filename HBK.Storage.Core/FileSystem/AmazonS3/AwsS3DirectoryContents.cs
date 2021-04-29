using Microsoft.Extensions.FileProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.AmazonS3
{
    /// <summary>
    /// AWS S3 目錄內容
    /// </summary>
    public class AwsS3DirectoryContents : IDirectoryContents
    {
        private readonly List<AwsS3FileInfo> _files;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="files">檔案清單</param>
        public AwsS3DirectoryContents(IEnumerable<AwsS3FileInfo> files)
        {
            _files = files.ToList();
        }

        /// <summary>
        /// 取得檔案室否存在
        /// </summary>
        public bool Exists => (_files.Count > 0);

        /// <summary>
        /// 取得列舉器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
