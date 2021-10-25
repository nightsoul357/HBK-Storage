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
        /// <summary>
        /// 批次更新檔案實體
        /// </summary>
        /// <param name="updateFileEntities"></param>
        /// <returns></returns>
        public async Task<List<FileEntity>> UpdateBatchAsync(List<FileEntity> updateFileEntities)
        {
            List<FileEntity> result = new List<FileEntity>();
            foreach (var data in updateFileEntities)
            {
                var original = await this.FindByIdAsync(data.FileEntityId);
                original.ExtendProperty = data.ExtendProperty;
                original.IsMarkDelete = data.IsMarkDelete;
                original.MimeType = data.MimeType;
                original.Name = data.Name;
                original.Size = data.Size;
                original.Status = data.Status;
                result.Add(original);
            }
            await _dbContext.SaveChangesAsync();
            return result;
        }
        /// <summary>
        /// 取得指定檔案的所有子檔案清單(遞迴查詢)
        /// </summary>
        /// <param name="rootFileEntityId">根檔案 ID</param>
        /// <returns></returns>
        public IQueryable<ChildFileEntity> GetChildFileEntitiesQuery(Guid rootFileEntityId)
        {
            var query = _dbContext.FileEntity
                .Include(x => x.FileEntityTag)
                .Join(
                _dbContext.VwFileEntityRecursive,
                fileEntity => fileEntity.FileEntityId,
                child => child.FileEntityId,
                (fileEntity, child) => new ChildFileEntity()
                {
                    ChildLevel = child.ChildLevel,
                    FileEntity = fileEntity,
                    RootFileEntityId = child.RootFileEntityId
                })
                .Where(x => x.RootFileEntityId == rootFileEntityId);

            return query;
        }
        #endregion
        #region BAL
        /// <summary>
        /// 取得檔案總存取次數
        /// </summary>
        /// <param name="fileEntityId">檔案 ID</param>
        /// <returns></returns>
        public Task<int> GetAccessTimesAsync(Guid fileEntityId)
        {
            return _dbContext.FileEntityStroageOperation
                .CountAsync(x => x.FileEntityStroage.FileEntityId == fileEntityId && x.Type == FileEntityStorageOperationTypeEnum.AccessSuccessfully);
        }
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
        /// 批次將檔案實體註記為刪除
        /// </summary>
        /// <param name="fileEntityIds">檔案實體 ID 清單</param>
        /// <returns></returns>
        public async Task MarkFileEntityDeleteBatchAsync(List<Guid> fileEntityIds)
        {
            foreach (var fileEntityId in fileEntityIds)
            {
                var fileEntity = await _dbContext.FileEntity
                .Include(x => x.FileEntityStroage)
                .FirstOrDefaultAsync(x => x.FileEntityId == fileEntityId);

                await this.MarkFileEntityDeleteInternalAsync(fileEntity);
            }

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
        /// 取得檔案存在的儲存個體橋接資訊集合
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns></returns>
        public Task<List<FileEntityStorage>> GetFileEntityStroageAsync(Guid fileEntityId)
        {
            return _dbContext.FileEntityStorage
                .Include(x => x.Storage)
                .ThenInclude(x => x.StorageGroup)
                .Where(x => x.FileEntityId == fileEntityId)
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
        /// <summary>
        /// 取得檔案實體所屬的儲存服務 ID
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns></returns>
        public async Task<Guid> GetStorageProviderIdByFileEntityIdAsync(Guid fileEntityId)
        {
            return (await _dbContext.FileEntity
                .Where(x => x.FileEntityId == fileEntityId)
                .Select(x => x.FileEntityStroage.First().Storage.StorageGroup.StorageProviderId)
                .FirstAsync()).Value;
        }
        /// <summary>
        /// 附加標籤到檔案實體上
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="tag">標籤</param>
        /// <returns></returns>
        public Task AppendTagAsync(Guid fileEntityId, string tag)
        {
            _dbContext.FileEntityTag.Add(new FileEntityTag()
            {
                FileEntityId = fileEntityId,
                Value = tag
            });
            return _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 移除檔案實體上的標籤
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="tag">標籤</param>
        /// <returns></returns>
        public async Task RemoveTagAsync(Guid fileEntityId, string tag)
        {
            var fileEntityTag = await _dbContext.FileEntityTag.FirstOrDefaultAsync(x => x.FileEntityId == fileEntityId && x.Value == tag);
            if (fileEntityTag != null)
            {
                _dbContext.FileEntityTag.Remove(fileEntityTag);
                await _dbContext.SaveChangesAsync();
            }
        }
        /// <summary>
        /// 取得子檔案實體清單
        /// </summary>
        /// <param name="parentFileEntityId">父檔案實體 ID</param>
        /// <returns></returns>
        public Task<List<FileEntity>> GetChildFileEntitiesAsync(Guid parentFileEntityId)
        {
            return this.ListQuery().Where(x => x.ParentFileEntityID == parentFileEntityId)
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
