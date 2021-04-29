using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 存取檔案任務模型
    /// </summary>
    public class FileAccessTaskModel
    {
        /// <summary>
        /// 取得或設定檔案服務 ID
        /// </summary>
        public Guid StorageProviderId { get; set; }
        /// <summary>
        /// 取得或設定檔案實體
        /// </summary>
        public FileEntity FileEntity { get; set; }
        /// <summary>
        /// 取得或設定檔案資訊
        /// </summary>
        public IAsyncFileInfo FileInfo { get; set; }
        /// <summary>
        /// 取得或設定存取用的權杖
        /// </summary>
        public JwtSecurityToken Token { get; set; }
    }
}
