using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.StorageCredentials;
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
    /// 儲存個體服務
    /// </summary>
    public class StorageService
    {
        private readonly HBKStorageContext _dbContext;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="dbContext">資料庫實體</param>
        public StorageService(HBKStorageContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region DAL

        /// <summary>
        /// 取得儲存個體列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<Adapter.Storages.Storage> ListQuery()
        {
            return _dbContext.Storage
                .Include(x => x.StorageGroup);
        }
        /// <summary>
        /// 以 ID 取得儲存個體
        /// </summary>
        /// <param name="storageId">儲存個體 ID</param>
        /// <returns></returns>
        public Task<Adapter.Storages.Storage> FindByIdAsync(Guid storageId)
        {
            return this.ListQuery()
                .FirstOrDefaultAsync(x => x.StorageId == storageId);
        }
        /// <summary>
        /// 新增儲存個體
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public async Task<Adapter.Storages.Storage> AddAsync(Adapter.Storages.Storage storage)
        {
            _dbContext.Storage.Add(storage);
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(storage.StorageId);
        }
        /// <summary>
        /// 更新儲存個體
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<Adapter.Storages.Storage> UpdateAsync(Adapter.Storages.Storage data)
        {
            var original = await _dbContext.Storage.FirstAsync(x => x.StorageId == data.StorageId);
            original.Credentials = data.Credentials;
            original.Name = data.Name;
            original.SizeLimit = data.SizeLimit;
            original.Status = data.Status;
            original.StorageGroupId = data.StorageGroupId;
            original.Type = data.Type;
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(original.StorageId);
        }
        /// <summary>
        /// 刪除儲存個體(同時會刪除所有儲存個體內的檔案)
        /// </summary>
        /// <param name="storageId">儲存個體 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid storageId)
        {
            _ = await _dbContext.Database
                .ExecuteSqlRawAsync($"Update FileEntityStorage Set IsMarkDelete = true Where StorgeID = '{storageId}'");
            _dbContext.Storage.Remove(await _dbContext.Storage.FirstAsync(x => x.StorageId == storageId));
            await _dbContext.SaveChangesAsync();
        }
        #endregion
        #region BAL
        /// <summary>
        /// 取得儲存個體延展資訊
        /// </summary>
        /// <param name="storageId">儲存個體 ID</param>
        /// <returns></returns>
        public Task<StorageExtendProperty> GetStorageExtendPropertyAsync(Guid storageId)
        {
            return this.ListQuery().Where(x => x.StorageId == storageId).Select(x => new StorageExtendProperty()
            {
                Storage = x,
                RemainSize = x.SizeLimit - x.FileEntityStroage.Sum(f => f.FileEntity.Size)
            }).FirstOrDefaultAsync();
        }
        /// <summary>
        /// 於儲存個體內新增檔案實體
        /// </summary>
        /// <param name="fileEntityStroage">檔案位於儲存個體上的橋接資訊</param>
        /// <returns></returns>
        public async Task<FileEntityStorage> AddFileEntityInStorageAsync(FileEntityStorage fileEntityStroage)
        {
            _dbContext.FileEntityStorage.Add(fileEntityStroage);
            await _dbContext.SaveChangesAsync();
            return fileEntityStroage;
        }
        /// <summary>
        /// 完成同步
        /// </summary>
        /// <param name="fileEntityStroageId">檔案位於儲存個體上的橋接資訊 ID</param>
        /// <param name="value">檔案儲存個體更新值</param>
        /// <returns></returns>
        public async Task CompleteSyncAsync(Guid fileEntityStroageId, string value)
        {
            var fileEntityStorage = await _dbContext.FileEntityStorage.FirstOrDefaultAsync(x => x.FileEntityStorageId == fileEntityStroageId);
            fileEntityStorage.Value = value;
            fileEntityStorage.Status = fileEntityStorage.Status & ~FileEntityStorageStatusEnum.Syncing;
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 撤銷同步(同步失敗)
        /// </summary>
        /// <param name="fileEntityStroageId">檔案位於儲存個體上的橋接資訊 ID</param>
        /// <returns></returns>
        public async Task RevorkSyncAsync(Guid fileEntityStroageId)
        {
            var fileEntityStorage = await _dbContext.FileEntityStorage.FirstOrDefaultAsync(x => x.FileEntityStorageId == fileEntityStroageId);
            fileEntityStorage.Status = FileEntityStorageStatusEnum.SyncFail;
            await _dbContext.SaveChangesAsync();

            _dbContext.FileEntityStorage.Remove(fileEntityStorage);
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 更新儲存個體驗證資訊
        /// </summary>
        /// <param name="storageId">儲存個體 ID</param>
        /// <param name="storageCredentialsBase">儲存個體驗證資訊</param>
        /// <returns></returns>
        public async Task<Adapter.Storages.Storage> UpdateCredentialsAsync(Guid storageId, StorageCredentialsBase storageCredentialsBase)
        {
            var storage = await _dbContext.Storage.FirstAsync(x => x.StorageId == storageId);
            storage.Credentials = storageCredentialsBase;
            await _dbContext.SaveChangesAsync();
            return storage;
        }
        #endregion
    }
}
