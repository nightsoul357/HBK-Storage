using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Services
{
    /// <summary>
    /// 檔案位於儲存個體上的資訊服務
    /// </summary>
    public class FileEntityStorageService
    {
        private readonly HBKStorageContext _dbContext;
        private readonly FileSystemFactory _fileSystemFactory;
        private readonly ILogger<FileEntityStorageService> _logger;
        private readonly FileEntityStorageServiceOption _option;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="dbContext">資料庫實體</param>
        /// <param name="fileSystemFactory">檔案系統工廠</param>
        /// <param name="logger"></param>
        /// <param name="option"></param>
        public FileEntityStorageService(HBKStorageContext dbContext, FileSystemFactory fileSystemFactory, ILogger<FileEntityStorageService> logger, FileEntityStorageServiceOption option)
        {
            _dbContext = dbContext;
            _fileSystemFactory = fileSystemFactory;
            _logger = logger;
            _option = option;
        }

        #region DAL

        /// <summary>
        /// 取得檔案位於儲存個體上的資訊列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<FileEntityStorage> ListQuery()
        {
            return _dbContext.FileEntityStorage
                .Include(x => x.Storage);
        }
        /// <summary>
        /// 以 ID 取得檔案位於儲存個體上的資訊
        /// </summary>
        /// <param name="fileEntityStorageId">檔案位於儲存個體上的資訊 ID</param>
        /// <returns></returns>
        public Task<FileEntityStorage> FindByIdAsync(Guid fileEntityStorageId)
        {
            return this.ListQuery()
                .FirstOrDefaultAsync(x => x.FileEntityStorageId == fileEntityStorageId);
        }
        /// <summary>
        /// 更新檔案位於儲存個體上的資訊
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<FileEntityStorage> UpdateAsync(FileEntityStorage data)
        {
            var original = await this.FindByIdAsync(data.FileEntityStorageId);
            original.CreatorIdentity = data.CreatorIdentity;
            original.IsMarkDelete = data.IsMarkDelete;
            original.Status = data.Status;
            original.Value = data.Value;
            await _dbContext.SaveChangesAsync();
            return original;
        }
        #endregion

        #region BAL

        /// <summary>
        /// 嘗試取得檔案資訊
        /// </summary>
        /// <param name="fileEntityStorageId">檔案位於儲存個體上的資訊 ID</param>
        /// <param name="ignoreStorageSoftDelete">是否忽略儲存個體已被刪除</param>
        /// <returns></returns>
        public async Task<IAsyncFileInfo> TryFetchFileInfoAsync(Guid fileEntityStorageId, bool ignoreStorageSoftDelete = false)
        {
            FileEntityStorage fileEntityStorage = null;
            if (ignoreStorageSoftDelete)
            {
                fileEntityStorage = await this.ListQuery()
                   .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.FileEntityStorageId == fileEntityStorageId && x.DeleteDateTime == null);
            }
            else
            {
                fileEntityStorage = await this.ListQuery()
                .FirstOrDefaultAsync(x => x.FileEntityStorageId == fileEntityStorageId);
            }

            var fileProvider = _fileSystemFactory.GetAsyncFileProvider(fileEntityStorage.Storage);
            try
            {
                var fileInfo = await fileProvider.GetFileInfoAsync(fileEntityStorage.Value);
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException();
                }

                await this.AddFetchSuccessfullyRecordAsync(fileEntityStorageId, String.Empty);
                return fileInfo;
            }
            catch (Exception ex)
            {
                await this.AddFetchFailRecordAsync(fileEntityStorageId, ex.Message);
                return default;
            }
        }
        /// <summary>
        /// 新增檔案位於儲存個體上的實體取得資訊失敗紀錄
        /// </summary>
        /// <param name="fileEntityStorageId">檔案位於儲存個體上的橋接資訊 ID</param>
        /// <param name="message">訊息</param>
        public async Task AddFetchFailRecordAsync(Guid fileEntityStorageId, string message)
        {
            _dbContext.FileEntityStroageOperation.Add(new FileEntityStroageOperation()
            {
                FileEntityStroageId = fileEntityStorageId,
                Message = message,
                Type = FileEntityStorageOperationTypeEnum.FetchFail,
                Status = FileEntityStorageOperationStatusEnum.None
            });
            await _dbContext.SaveChangesAsync();

            var fileEntityStorage = await _dbContext.FileEntityStorage
                .Include(x => x.FileEntity)
                .Include(x => x.Storage)
                .ThenInclude(x => x.StorageGroup)
                .FirstOrDefaultAsync(x => x.FileEntityStorageId == fileEntityStorageId);

            var lastFetchSuccessfullyDateTime = _dbContext.FileEntityStroageOperation
                    .Where(x => x.FileEntityStroageId == fileEntityStorageId && x.Type == FileEntityStorageOperationTypeEnum.FetchSuccessfully)
                    .Select(x => x.CreateDateTime)
                    .DefaultIfEmpty()
                    .Max();

            var fetchFailCount = _dbContext.FileEntityStroageOperation
                .Count(x => x.FileEntityStroageId == fileEntityStorageId && x.Type == FileEntityStorageOperationTypeEnum.FetchFail && x.CreateDateTime >= lastFetchSuccessfullyDateTime);
            if (fetchFailCount >= _option.FetchFailToCloseFileCount)
            {
                fileEntityStorage.Status = fileEntityStorage.Status | FileEntityStorageStatusEnum.Disable;
                _logger.LogWarning("檔案 ID 為 {0} 的檔案 {1} 在 {2} 儲存群組中的 {3} 儲存個體 Fetch 時失敗超過 {4} 次，故該檔案已被關閉",
                    fileEntityStorage.FileEntity.FileEntityId,
                    fileEntityStorage.FileEntity.Name,
                    fileEntityStorage.Storage.StorageGroup.Name,
                    fileEntityStorage.Storage.Name,
                    _option.FetchFailToCloseFileCount);
            }

            var lastStorageFetchSuccessfullyDateTime = _dbContext.FileEntityStroageOperation
                .Where(x => x.FileEntityStroage.StorageId == fileEntityStorage.StorageId && x.Type == FileEntityStorageOperationTypeEnum.FetchSuccessfully)
                .Select(x => x.CreateDateTime)
                .DefaultIfEmpty()
                .Max();
            var storageFetchFailCount = _dbContext.FileEntityStroageOperation
                .Count(x => x.FileEntityStroage.StorageId == fileEntityStorage.StorageId && x.Type == FileEntityStorageOperationTypeEnum.FetchFail && x.CreateDateTime >= lastStorageFetchSuccessfullyDateTime);
            if (storageFetchFailCount >= _option.FetchFailToCloseStorgeCount)
            {
                fileEntityStorage.Storage.Status = fileEntityStorage.Storage.Status | StorageStatusEnum.Disable;
                _logger.LogWarning("在儲存個體 {0} Fetch 檔案失敗超過 {1} 次，故該儲存個體已被關閉",
                    fileEntityStorage.Storage.Name,
                    _option.FetchFailToCloseStorgeCount);
            }

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 新增檔案位於儲存個體上的實體取得資訊成功紀錄
        /// </summary>
        /// <param name="fileEntityStorageId">檔案位於儲存個體上的橋接資訊 ID</param>
        /// <param name="message">訊息</param>
        public Task AddFetchSuccessfullyRecordAsync(Guid fileEntityStorageId, string message)
        {
            _dbContext.FileEntityStroageOperation.Add(new FileEntityStroageOperation()
            {
                FileEntityStroageId = fileEntityStorageId,
                Message = message,
                Type = FileEntityStorageOperationTypeEnum.FetchSuccessfully,
                Status = FileEntityStorageOperationStatusEnum.None
            });
            return _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 新增檔案位於儲存個體上的實體同步失敗紀錄
        /// </summary>
        /// <param name="fileEntityStorageId">檔案位於儲存個體上的橋接資訊 ID</param>
        /// <param name="message">訊息</param>
        /// <param name="syncTargeStorageId">同步目標儲存個體 ID</param>
        public async Task AddSyncFailRecordAsync(Guid fileEntityStorageId, string message, Guid syncTargeStorageId)
        {
            _dbContext.FileEntityStroageOperation.Add(new FileEntityStroageOperation()
            {
                FileEntityStroageId = fileEntityStorageId,
                Message = message,
                Type = FileEntityStorageOperationTypeEnum.SyncFail,
                Status = FileEntityStorageOperationStatusEnum.None,
                SyncTargetStorageId = syncTargeStorageId
            });
            await _dbContext.SaveChangesAsync();

            var lastSyncSuccessfullyDateTime = _dbContext.FileEntityStroageOperation
                .Where(x => x.SyncTargetStorageId == syncTargeStorageId && x.Type == FileEntityStorageOperationTypeEnum.SyncSuccessfully)
                .Select(x => x.CreateDateTime)
                .DefaultIfEmpty()
                .Max();
            var syncFailCount = _dbContext.FileEntityStroageOperation
                .Where(x => x.SyncTargetStorageId == syncTargeStorageId && x.Type == FileEntityStorageOperationTypeEnum.SyncFail && x.CreateDateTime >= lastSyncSuccessfullyDateTime)
                .Count();

            if (syncFailCount >= _option.SyncTargetFailtToCloseStorageCount)
            {
                var storage = (await _dbContext.Storage.FirstAsync(x => x.StorageId == syncTargeStorageId));
                storage.Status = storage.Status | StorageStatusEnum.Disable;
                await _dbContext.SaveChangesAsync();
                _logger.LogWarning("嘗試同步至 {0} 儲存個體失敗超過 {1} 次，故該儲存個體已被關閉",
                    storage.Name,
                    _option.SyncTargetFailtToCloseStorageCount);

            }
        }

        /// <summary>
        /// 新增檔案位於儲存個體上的實體同步成功紀錄
        /// </summary>
        /// <param name="fileEntityStorageId">檔案位於儲存個體上的橋接資訊 ID</param>
        /// <param name="message">訊息</param>
        /// <param name="syncTargeStorageId">同步目標儲存個體 ID</param>
        public Task AddSyncSuccessfullyRecordAsync(Guid fileEntityStorageId, string message, Guid syncTargeStorageId)
        {
            _dbContext.FileEntityStroageOperation.Add(new FileEntityStroageOperation()
            {
                FileEntityStroageId = fileEntityStorageId,
                Message = message,
                Type = FileEntityStorageOperationTypeEnum.SyncSuccessfully,
                Status = FileEntityStorageOperationStatusEnum.None,
                SyncTargetStorageId = syncTargeStorageId
            });
            return _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 新增檔案位於儲存個體上的實體存取失敗紀錄
        /// </summary>
        /// <param name="fileEntityStorageId">檔案位於儲存個體上的橋接資訊 ID</param>
        /// <param name="message">訊息</param>
        public Task AddSyncAccessFailRecordAsync(Guid fileEntityStorageId, string message)
        {
            _dbContext.FileEntityStroageOperation.Add(new FileEntityStroageOperation()
            {
                FileEntityStroageId = fileEntityStorageId,
                Message = message,
                Type = FileEntityStorageOperationTypeEnum.AccessFail,
                Status = FileEntityStorageOperationStatusEnum.None,
            });
            return _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 新增檔案位於儲存個體上的實體存取成功紀錄
        /// </summary>
        /// <param name="fileEntityStorageId">檔案位於儲存個體上的橋接資訊 ID</param>
        /// <param name="message">訊息</param>
        public Task AddAccessSuccessfullyRecordAsync(Guid fileEntityStorageId, string message)
        {
            _dbContext.FileEntityStroageOperation.Add(new FileEntityStroageOperation()
            {
                FileEntityStroageId = fileEntityStorageId,
                Message = message,
                Type = FileEntityStorageOperationTypeEnum.AccessSuccessfully,
                Status = FileEntityStorageOperationStatusEnum.None,
            });
            return _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 刪除同步中檔案位於儲存個體上的橋接資訊
        /// </summary>
        /// <param name="fileEntityNoDivisor">檔案實體流水號除數</param>
        /// <param name="fileEntityNoRemainder">檔案實體流水號餘數</param>
        /// <returns>移除數量</returns>
        public async Task<int> DeleteSyncingFileEntityStorageAsync(int fileEntityNoDivisor, int fileEntityNoRemainder)
        {
            var shouldRemove = await _dbContext.FileEntityStorage
                .Where(x => x.FileEntity.FileEntityNo % fileEntityNoDivisor == fileEntityNoRemainder && x.Status.HasFlag(FileEntityStorageStatusEnum.Syncing))
                .ToListAsync();

            shouldRemove.ForEach(x => x.IsMarkDelete = true); // TODO : 修改為批次作業

            await _dbContext.SaveChangesAsync();

            return shouldRemove.Count;
        }
        #endregion
    }
}
