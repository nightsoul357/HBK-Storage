using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.FileProcessHandlers;
using HBK.Storage.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers.Models
{
    /// <summary>
    /// 檔案存取中繼資料
    /// </summary>
    public class FileAccessMiddleData
    {
        /// <summary>
        /// 取得或設定 JWT
        /// </summary>
        public JwtSecurityToken JwtSecurityToken { get; set; }
        /// <summary>
        /// 取得或設定檔案實體
        /// </summary>
        public FileEntity FileEntity { get; set; }
        /// <summary>
        /// 取得或設定檔案存取權杖
        /// </summary>
        public FileAccessToken FileAccessToken { get; set; }
        /// <summary>
        /// 取得或設定檔案位於儲存個體上的橋接資訊
        /// </summary>
        public FileEntityStorage FileEntityStorage { get; set; }
        /// <summary>
        /// 取得或設定儲存服務檔案資訊
        /// </summary>
        public IAsyncFileInfo FileInfo { get; set; }
        /// <summary>
        /// 取得或設定處理檔案任務模型
        /// </summary>
        public FileProcessTaskModel FileProcessTaskModel { get; set; }
    }
}
