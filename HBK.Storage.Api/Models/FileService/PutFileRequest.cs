using HBK.Storage.Adapter.Enums;
using HBK.Storage.Api.ModelBinders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.FileService
{
    /// <summary>
    /// 上傳檔案的請求內容
    /// </summary>
    [ModelBinder(BinderType = typeof(PutFileRequestBinder))]
    public class PutFileRequest
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public PutFileRequest()
        {
            this.Tags = new List<string>();
            this.CryptoMode = CryptoModeEnum.NoCrypto;
        }
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// 強制指定上傳目標的儲存集合 ID
        /// </summary>
        public Guid? StorageGroupId { get; set; }
        /// <summary>
        /// 檔案的額外資訊
        /// </summary>
        public string ExtendProperty { get; set; }
        /// <summary>
        /// 檔案標籤清單
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 檔案的網際網路媒體型式，留空將會透過檔名自動判定
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// 加密方式
        /// </summary>
        public CryptoModeEnum CryptoMode { get; set; }
        /// <summary>
        /// 檔案
        /// </summary>
        public IFormFile File { get; set; } // 用於 Swagger 自動產生對應資訊
        /// <summary>
        /// 檔案串流
        /// </summary>
        internal Stream FileStream { get; set; }
    }
}