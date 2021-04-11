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
            return _dbContext.Storage;
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
        public async Task<FileEntityStroage> AddFileEntityInStorageAsync(FileEntityStroage fileEntityStroage)
        {
            _dbContext.FileEntityStroage.Add(fileEntityStroage);
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
            var fileEntityStorage = await _dbContext.FileEntityStroage.FirstOrDefaultAsync(x => x.FileEntityStroageId == fileEntityStroageId);
            fileEntityStorage.Value = value;
            fileEntityStorage.Status = fileEntityStorage.Status & ~Adapter.Enums.FileEntityStorageStatusEnum.Syncing;
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
