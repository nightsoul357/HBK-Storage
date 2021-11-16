using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.Memory
{
    /// <summary>
    /// 記憶體內的檔案資訊
    /// </summary>
    public class MemoryFileInfo : AsyncFileInfo
    {
        private MemoryStream _ms;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="ms"></param>
        public MemoryFileInfo(MemoryStream ms)
        {
            _ms = ms;
            this.Length = ms.Length;
        }

        /// <inheritdoc/>
        public override Task<Stream> CreateReadStreamAsync()
        {
            return Task<Stream>.Run(() => (Stream)_ms);
        }
    }
}
