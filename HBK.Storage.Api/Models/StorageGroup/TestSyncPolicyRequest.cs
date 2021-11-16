using HBK.Storage.Api.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.StorageGroup
{
    /// <summary>
    /// 測試同步策略請求內容
    /// </summary>
    public class TestSyncPolicyRequest
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        /// <example>test.mp4</example>
        public string Name { get; set; }
        /// <summary>
        /// 檔案大小(單位 Bytes)
        /// </summary>
        /// <example>104929280</example>
        public long Size { get; set; }
        /// <summary>
        /// 檔案擴充描述
        /// </summary>
        /// <example>This is some information...</example>
        public string ExtendProperty { get; set; }
        /// <summary>
        /// 檔案的網際網路媒體型式
        /// </summary>
        /// <example>video/mp4</example>
        public string MimeType { get; set; }
        /// <summary>
        /// 檔案的標籤清單
        /// </summary>
        /// <example>["video","hbk"]</example>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 規則
        /// </summary>
        public SyncPolicyRequest SyncPolicy { get; set; }
    }
}