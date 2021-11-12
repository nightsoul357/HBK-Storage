using CG.Web.MegaApiClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.Mega
{
    /// <summary>
    /// Mega 的檔案資訊
    /// </summary>
    public class MegaFileInfo : AsyncFileInfo
    {
        private readonly MegaApiClient _megaApiClient;
        private readonly INode _node;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="megaApiClient"></param>
        /// <param name="node"></param>
        public MegaFileInfo(MegaApiClient megaApiClient, INode node)
        {
            _megaApiClient = megaApiClient;
            _node = node;
            this.Name = node.Id;
            this.Length = node.Size;
            this.LastModified = node.ModificationDate ?? DateTimeOffset.MinValue;
            this.Exists = true;
            this.IsDirectory = node.Type == NodeType.Directory;
        }

        /// <inheritdoc/>
        public override Task<Stream> CreateReadStreamAsync()
        {
            if (!this.Exists)
            {
                throw new FileNotFoundException();
            }
            return _megaApiClient.DownloadAsync(_node);
        }
    }
}
