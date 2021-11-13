using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.Models;
using HBK.Storage.Core.Strategies;
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
        /// <summary>
        /// 新增儲存個體集合
        /// </summary>
        /// <param name="storageGroup"></param>
        /// <returns></returns>
        public async Task<StorageGroup> AddAsync(StorageGroup storageGroup)
        {
            _dbContext.StorageGroup.Add(storageGroup);
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(storageGroup.StorageGroupId);
        }
        /// <summary>
        /// 更新儲存個體集合
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<StorageGroup> UpdateAsync(StorageGroup data)
        {
            var original = await _dbContext.StorageGroup.FirstAsync(x => x.StorageGroupId == data.StorageGroupId);
            original.Name = data.Name;
            original.Status = data.Status;
            original.StorageProviderId = data.StorageProviderId;
            original.SyncMode = data.SyncMode;
            original.SyncPolicy = data.SyncPolicy;
            original.Type = data.Type;
            original.ClearPolicy = data.ClearPolicy;
            original.ClearMode = data.ClearMode;
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(original.StorageGroupId);
        }
        /// <summary>
        /// 刪除儲存個體集合(同時會刪除所有儲存個體)
        /// </summary>
        /// <param name="storageGroupId">儲存個體集合 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid storageGroupId)
        {
            var storageGroup = await _dbContext.StorageGroup
                .Include(x => x.Storage)
                .FirstAsync(x => x.StorageGroupId == storageGroupId);

            foreach (var storage in storageGroup.Storage)
            {
                await _storageService.DeleteAsync(storage.StorageId);
            }

            _dbContext.Remove(storageGroup);
            await _dbContext.SaveChangesAsync();
        }
        #endregion

        #region BAL

        /// <summary>
        /// 取得儲存群組擴充資訊清單
        /// </summary>
        /// <returns></returns>
        public IQueryable<StorageGroupExtendProperty> GetStorageGroupExtendPropertiesQuery()
        {
            var query = _dbContext.StorageGroup
                .Join(_dbContext.VwStorageGroupAnalysis, sg => sg.StorageGroupId, sga => sga.StorageGroupId, (sg, sga) => new StorageGroupExtendProperty()
                {
                    StorageGroup = sg,
                    UsedSize = sga.UsedSize,
                    SizeLimit = sga.SizeLimit
                });

            return query;
        }
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
        /// <param name="storageTypes">僅使用指定類型清單的儲存個體<c>null</c> 表示全部使用</param>
        /// <returns></returns>
        public async Task<StorageExtendProperty> GetMaxRemainSizeStorageByStorageGroupIdAsync(Guid storageGroupId, List<StorageTypeEnum> storageTypes = null)
        {
            if (storageTypes == null)
            {
                storageTypes = Enum.GetValues<StorageTypeEnum>().Cast<StorageTypeEnum>().ToList();
            }
            var storageGroup = await this.FindByIdAsync(storageGroupId);
            return storageGroup.Storage
                .Where(x => !x.Status.HasFlag(StorageStatusEnum.Disable) && storageTypes.Contains(x.Type))
                .Select(x => _storageService.GetStorageExtendPropertyAsync(x.StorageId).Result)
                .OrderByDescending(x => x.RemainSize)
                .FirstOrDefault();
        }
        #endregion
    }
}
