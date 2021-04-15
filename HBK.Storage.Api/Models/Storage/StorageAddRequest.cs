using HBK.Storage.Api.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Models.Storage
{
    /// <summary>
    /// 新增儲存個體請求內容
    /// </summary>
    public class StorageAddRequest : StorageUpdateRequest
    {
        /// <summary>
        /// 儲存個體集合 ID
        /// </summary>
        /// <example>8acdbf86-cb7b-4d1a-8745-44115f656287</example>
        [ExistInDatabase(typeof(Adapter.Storages.StorageGroup))]
        public Guid StorageGroupId { get; set; }
    }
}
