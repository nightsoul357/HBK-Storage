using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Services
{
    /// <summary>
    /// 儲存個體群組服務
    /// </summary>
    public class StorageGroupService
    {
        private readonly HBKStorageContext _dbContext;
        private readonly StorageService _storageService;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="dbContext">資料庫實體</param>
        /// <param name="storageService">儲存個體服務</param>
        public StorageGroupService(HBKStorageContext dbContext, StorageService storageService)
        {
            _dbContext = dbContext;
            _storageService = storageService;
        }

        #region DAL
        /// <summary>
        /// 取得儲存個體群組列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<StorageGroup> ListQuery()
        {
            return _dbContext.StorageGroup
                .Include(x => x.Storage);
        }
        /// <summary>
        /// 以 ID 取得儲存個體群組
        /// </summary>
        /// <param name="storageGroupId">儲存個體群組 ID</param>
        /// <returns></returns>
        public Task<StorageGroup> FindByIdAsync(Guid storageGroupId)
        {
            return this.ListQuery()
                .FirstOrDefaultAsync(x => x.StorageGroupId == storageGroupId);
        }
        #endregion

        #region BAL
        /// <summary>
        /// 停用指定的儲存個體集合
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <returns></returns>
        public async Task DisableStorageGroupAsync(Guid storageGroupId)
        {
            var storageGroup = await this.FindByIdAsync(storageGroupId);
            storageGroup.Status = storageGroup.Status | StorageGroupStatusEnum.Disable;
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 取得儲存個體群組內剩餘容量最多的儲存個體延展資訊
        /// </summary>
        /// <param name="storageGroupId">儲存個體群組 ID</param>
        /// <returns></returns>
        public async Task<StorageExtendProperty> GetMaxRemainSizeStorageByStorageGroupIdAsync(Guid storageGroupId)
        {
            var storageGroup = await this.FindByIdAsync(storageGroupId);
            return storageGroup.Storage
                .Where(x => !x.Status.HasFlag(StorageStatusEnum.Disable))
                .Select(x => _storageService.GetStorageExtendPropertyAsync(x.StorageId).Result)
                .OrderByDescending(x => x.RemainSize)
                .FirstOrDefault();
        }
        #endregion
    }
}
