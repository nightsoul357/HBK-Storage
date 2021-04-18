using HBK.Storage.Adapter.Storages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Services
{
    /// <summary>
    /// 檔案實體服務
    /// </summary>
    public class FileEntityService
    {
        private readonly HBKStorageContext _dbContext;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="dbContext">資料庫實體</param>
        public FileEntityService(HBKStorageContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region DAL
        /// <summary>
        /// 取得檔案實體列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<FileEntity> ListQuery()
        {
            return _dbContext.FileEntity
                .Include(x => x.FileEntityTag);
        }
        /// <summary>
        /// 以 ID 取得檔案實體
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns></returns>
        public Task<FileEntity> FindByIdAsync(Guid fileEntityId)
        {
            return this.ListQuery()
                .FirstOrDefaultAsync(x => x.FileEntityId == fileEntityId);
        }
        /// <summary>
        /// 更新檔案實體
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<FileEntity> UpdateAsync(FileEntity data)
        {
            var original = await this.FindByIdAsync(data.FileEntityId);
            original.ExtendProperty = data.ExtendProperty;
            original.IsMarkDelete = data.IsMarkDelete;
            original.MimeType = data.MimeType;
            original.Name = data.Name;
            original.Size = data.Size;
            original.Status = data.Status;
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(data.FileEntityId);
        }
        #endregion
        #region BAL
        /// <summary>
        /// 將檔案實體註記為刪除
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns></returns>
        public async Task MarkFileEntityDeleteAsync(Guid fileEntityId)
        {
            var fileEntity = await _dbContext.FileEntity
                .Include(x => x.FileEntityStroage)
                .FirstOrDefaultAsync(x => x.FileEntityId == fileEntityId);
            await this.MarkFileEntityDeleteInternalAsync(fileEntity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 取得需要被刪除的檔案位於儲存個體上的橋接資訊清單
        /// </summary>
        /// <param name="takeCount">取得數量</param>
        /// <param name="fileEntityNoDivisor">檔案實體流水號除數</param>
        /// <param name="fileEntityNoRemainder">檔案實體流水號餘數</param>
        /// <returns></returns>
        public Task<List<FileEntityStorage>> GetMarkDeleteFileEntityStoragiesAsync(int takeCount, int fileEntityNoDivisor, int fileEntityNoRemainder)
        {
            return _dbContext.FileEntityStorage
                .Include(x => x.FileEntity)
                .Include(x => x.Storage)
                .ThenInclude(x => x.StorageGroup)
                .Where(x => x.IsMarkDelete && x.FileEntity.FileEntityNo % fileEntityNoDivisor == fileEntityNoRemainder)
                .Take(takeCount)
                .ToListAsync();
        }
        /// <summary>
        /// 更新移除完成的檔案實體
        /// </summary>
        /// <param name="updateCount">更新數量</param>
        /// <param name="fileEntityNoDivisor">檔案實體流水號除數</param>
        /// <param name="fileEntityNoRemainder">檔案實體流水號餘數</param>
        /// <returns></returns>
        public async Task<int> UpdateFileEntityDeleteInfoAsync(int updateCount, int fileEntityNoDivisor, int fileEntityNoRemainder)
        {
            var deleteFileEntities = _dbContext.FileEntity
                .Where(x => x.IsMarkDelete && x.FileEntityNo % fileEntityNoDivisor == fileEntityNoRemainder && x.FileEntityStroage.Count == 0)
                .Take(updateCount)
                .ToList();
            _dbContext.FileEntity.RemoveRange(deleteFileEntities);
            await _dbContext.SaveChangesAsync();
            return deleteFileEntities.Count;
        }
        /// <summary>
        /// 刪除檔案位於儲存個體上的橋接資訊
        /// </summary>
        /// <param name="fileEntityStorageId">儲存個體上的橋接資訊 ID</param>
        /// <returns></returns>
        public async Task DeleteFileEntityStroageAsync(Guid fileEntityStorageId)
        {
            var fileEntityStorage = await _dbContext.FileEntityStorage
                .Include(x => x.FileEntity)
                .ThenInclude(x => x.FileEntityStroage)
                .FirstAsync(x => x.FileEntityStorageId == fileEntityStorageId);

            if (fileEntityStorage.FileEntity.FileEntityStroage.Count <= 1)
            {
                _dbContext.FileEntity.Remove(fileEntityStorage.FileEntity);
            }

            _dbContext.FileEntityStorage
               .Remove(await _dbContext.FileEntityStorage.FirstAsync(x => x.FileEntityStorageId == fileEntityStorageId));
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 取得檔案存在的儲存個體集合
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns></returns>
        public Task<List<Adapter.Storages.Storage>> GetStoragesAsync(Guid fileEntityId)
        {
            return _dbContext.Storage
                .Include(x => x.StorageGroup)
                .Where(x => x.FileEntityStroage.Any(f => f.FileEntityId == fileEntityId))
                .ToListAsync();
        }
        /// <summary>
        /// 取得已過期但未標記刪除的檔案
        /// </summary>
        /// <param name="takeCount">取得數量</param>
        /// <param name="fileEntityNoDivisor">檔案實體流水號除數</param>
        /// <param name="fileEntityNoRemainder">檔案實體流水號餘數</param>
        /// <returns></returns>
        public Task<List<FileEntity>> GetExpireFileEntityWithoutMarkDeleteAsync(int takeCount, int fileEntityNoDivisor, int fileEntityNoRemainder)
        {
            return _dbContext.FileEntity
                .Where(x => !x.IsMarkDelete &&
                    x.FileEntityNo % fileEntityNoDivisor == fileEntityNoRemainder &&
                    DateTimeOffset.Now >= x.ExpireDateTime)
                .Take(takeCount)
                .ToListAsync();
        }
        #endregion
        #region private method
        private async Task MarkFileEntityDeleteInternalAsync(FileEntity fileEntity)
        {
            var child = await _dbContext.FileEntity
                .Include(x => x.FileEntityStroage)
                .Where(x => x.ParentFileEntityID == fileEntity.FileEntityId)
                .ToListAsync();

            if (child.Count != 0)
            {
                foreach (var file in child)
                {
                    await this.MarkFileEntityDeleteInternalAsync(file);
                }
            }

            fileEntity.IsMarkDelete = true;
            fileEntity.FileEntityStroage.ToList().ForEach(x => x.IsMarkDelete = true);
        }
        #endregion
    }
}
