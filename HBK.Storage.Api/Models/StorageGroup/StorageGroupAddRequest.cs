using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Models;
using HBK.Storage.Api.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.StorageGroup
{
    /// <summary>
    /// 新增儲存個體集合請求內容
    /// </summary>
    public class StorageGroupAddRequest : StorageGroupUpdateRequest
    {
        /// <summary>
        /// 所屬儲存服務 ID
        /// </summary>
        /// <example>59b50410-e86a-4341-8973-ae325e354210</example>
        [ExistInDatabase(typeof(Adapter.Storages.StorageProvider))]
        public Guid StorageProviderId { get; set; }
    }
}
