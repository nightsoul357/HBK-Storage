using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.FileProcessHandlers;
using HBK.Storage.Core.FileSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers.Models
{
    /// <summary>
    /// 存取檔案任務模型
    /// </summary>
    public class FileAccessTaskModel
    {
        /// <summary>
        /// 初始化存取檔案任務模型
        /// </summary>
        /// <param name="esic"></param>
        public FileAccessTaskModel(string esic)
        {
            this.Request = new FileAccessRequest()
            {
                Esic = esic
            };
            this.MiddleData = new FileAccessMiddleData();
        }
        /// <summary>
        /// 初始化存取檔案任務模型
        /// </summary>
        /// <param name="fileEntityId"></param>
        /// <param name="esic"></param>
        public FileAccessTaskModel(Guid fileEntityId, string esic)
        {
            this.Request = new FileAccessRequest()
            {
                FileEntityId = fileEntityId,
                Esic = esic
            };
            this.MiddleData = new FileAccessMiddleData();
        }
        /// <summary>
        /// 初始化存取檔案任務模型
        /// </summary>
        /// <param name="fileEntityId"></param>
        public FileAccessTaskModel(Guid fileEntityId)
        {
            this.Request = new FileAccessRequest()
            {
                FileEntityId = fileEntityId,
            };
            this.MiddleData = new FileAccessMiddleData();
        }
        /// <summary>
        /// 取得或設定檔案處理請求內容
        /// </summary>
        public FileAccessRequest Request { get; set; }
        /// <summary>
        /// 取得或設定檔案存取中繼資料
        /// </summary>
        public FileAccessMiddleData MiddleData { get; set; }
        /// <summary>
        /// 取得或設定錯誤回應內容
        /// </summary>
        public object ErrorObject { get; set; }
        /// <summary>
        /// 取得或設定回應檔案
        /// </summary>
        public IAsyncFileInfo FileInfo { get; set; }
    }
}
