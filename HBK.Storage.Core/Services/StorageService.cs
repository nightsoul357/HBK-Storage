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
        #endregion
    }
}
