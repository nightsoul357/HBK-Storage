using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.Exceptions;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Models;
using HBK.Storage.Core.Strategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Services
{
    /// <summary>
    /// 儲存服務服務
    /// </summary>
    public class StorageProviderService
    {
        private readonly HBKStorageContext _dbContext;
        private readonly StorageGroupService _storageGroupService;
        private readonly FileSystemFactory _fileSystemFactory;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="dbContext">資料庫實體</param>
        /// <param name="storageGroupService">儲存個體群組服務</param>
        /// <param name="fileSystemFactory">檔案系統工廠</param>
        public StorageProviderService(HBKStorageContext dbContext, StorageGroupService storageGroupService, FileSystemFactory fileSystemFactory)
        {
            _dbContext = dbContext;
            _storageGroupService = storageGroupService;
            _fileSystemFactory = fileSystemFactory;
        }

        #region DAL
        /// <summary>
        /// 取得儲存服務列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<StorageProvider> ListQuery()
        {
            return _dbContext.StorageProvider
                .Include(x => x.StorageGroup)
                .ThenInclude(x => x.Storage);
        }
        /// <summary>
        /// 以 ID 取得儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <returns></returns>
        public Task<StorageProvider> FindByIdAsync(Guid storageProviderId)
        {
            return this.ListQuery()
                .FirstOrDefaultAsync(x => x.StorageProviderId == storageProviderId);
        }
        /// <summary>
        /// 取得所有儲存服務
        /// </summary>
        /// <returns></returns>
        public Task<List<StorageProvider>> GetAllStorageProviderAsync()
        {
            return this.ListQuery().ToListAsync();
        }
        /// <summary>
        /// 取得指定儲存服務 ID 清單的所有儲存服務
        /// </summary>
        /// <param name="storageProviderIds">儲存服務 ID 清單</param>
        /// <returns></returns>
        public Task<List<StorageProvider>> GetStorageProviderByIdsAsync(List<Guid> storageProviderIds)
        {
            return this.ListQuery().Where(x => storageProviderIds.Contains(x.StorageProviderId)).ToListAsync();
        }
        /// <summary>
        /// 加入儲存服務
        /// </summary>
        /// <param name="storageProvider"></param>
        /// <returns></returns>
        public async Task<StorageProvider> AddAsync(StorageProvider storageProvider)
        {
            _dbContext.StorageProvider.Add(storageProvider);
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(storageProvider.StorageProviderId);
        }
        /// <summary>
        /// 更新儲存服務
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<StorageProvider> UpdateAsync(StorageProvider data)
        {
            var original = await this.FindByIdAsync(data.StorageProviderId);
            original.Name = data.Name;
            original.Status = data.Status;
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(data.StorageProviderId);
        }
        /// <summary>
        /// 刪除儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid storageProviderId)
        {
            var storageProvider = await _dbContext.StorageProvider
                .Include(x => x.StorageGroup)
                .FirstAsync(x => x.StorageProviderId == storageProviderId);
            foreach (var storageGroup in storageProvider.StorageGroup)
            {
                await _storageGroupService.DeleteAsync(storageGroup.StorageGroupId);
            }
            _dbContext.StorageProvider.Remove(storageProvider);
            await _dbContext.SaveChangesAsync();
        }
        #endregion

        #region BAL
        /// <summary>
        /// 上傳檔案至指定的儲存服務
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="storageGroupId">強制指定儲存集合 ID</param>
        /// <param name="fileEntity">檔案實體</param>
        /// <param name="fileStream">檔案流</param>
        /// <param name="creatorIdentity">建立者識別</param>
        /// <param name="storageTypes">僅使用指定類型清單的儲存個體<c>null</c> 表示全部使用</param>
        /// <returns></returns>
        public async Task<FileEntity> UploadFileEntityAsync(Guid storageProviderId, Guid? storageGroupId, FileEntity fileEntity, Stream fileStream, string creatorIdentity, List<StorageTypeEnum> storageTypes = null)
        {
            if (storageTypes == null)
            {
                storageTypes = Enum.GetValues<StorageTypeEnum>().Cast<StorageTypeEnum>().ToList();
            }
            var storageProvider = await this.FindByIdAsync(storageProviderId);
            StorageGroup storageGroup = null;
            StorageExtendProperty storageExtendProperty = null;

            #region 取得對應 Storage Group
            if (storageGroupId != null)
            {
                storageGroup = storageProvider.StorageGroup.FirstOrDefault(x => x.StorageGroupId == storageGroupId.Value);
                storageExtendProperty = await _storageGroupService.GetMaxRemainSizeStorageByStorageGroupIdAsync(storageGroup.StorageGroupId, storageTypes);
            }
            else if (storageGroupId == null)
            {
                var sg = storageProvider.StorageGroup
                    .Where(x => !x.Status.HasFlag(StorageGroupStatusEnum.Disable))
                    .Select(x =>
                        new
                        {
                            StorageGroup = x,
                            StoragExtendProperty = _storageGroupService.GetMaxRemainSizeStorageByStorageGroupIdAsync(x.StorageGroupId, storageTypes).Result
                        })
                    .Where(x => x.StoragExtendProperty?.RemainSize >= fileEntity.Size) // 過濾掉剩餘空間不足
                    .OrderByDescending(x => x.StorageGroup.UploadPriority) // 先根據上傳優先度排序
                    .ThenByDescending(x => x.StoragExtendProperty?.RemainSize) // 再根據剩餘空間排序
                    .ToList();

                if (sg.Count == 0)
                {
                    throw new InvalidOperationException($"Couldn't find any valid storage group.");
                }

                var sgMain = sg.FirstOrDefault();

                storageGroup = sgMain.StorageGroup;
                storageExtendProperty = sgMain.StoragExtendProperty;
            }

            if (storageGroup.Status.HasFlag(StorageGroupStatusEnum.Disable))
            {
                throw new InvalidOperationException($"Storage Group { storageGroup.Name } is disabled.");
            }
            if (storageExtendProperty == null || storageExtendProperty.RemainSize <= fileEntity.Size)
            {
                throw new InvalidOperationException("Remain space not enough.");
            }
            #endregion
            Guid taskId = Guid.NewGuid();
            fileEntity.Status = FileEntityStatusEnum.Uploading | fileEntity.Status;

            _dbContext.FileEntity.Add(fileEntity);
            await _dbContext.SaveChangesAsync();

            var fileProvider = _fileSystemFactory.GetAsyncFileProvider(storageExtendProperty.Storage);
            var fileInfoResult = await fileProvider.PutAsync(taskId.ToString(), fileStream);

            var fileEntityStorage = new FileEntityStorage()
            {
                CreatorIdentity = creatorIdentity,
                Status = FileEntityStorageStatusEnum.None,
                StorageId = storageExtendProperty.Storage.StorageId,
                Value = fileInfoResult.Name
            };
            fileEntity.FileEntityStroage.Add(fileEntityStorage);
            fileEntity.Status = fileEntity.Status & ~FileEntityStatusEnum.Uploading;
            fileEntity.Size = fileInfoResult.Length;

            await _dbContext.SaveChangesAsync();
            return fileEntity;
        }

        /// <summary>
        /// 取得指定儲存服務內的指定檔案實體的橋接資訊
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="storageGroupId">強制指定儲存個體群組 ID</param>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="storageTypes">指定有效的儲存服務類型清單，<c>null</c> 表示指定所有</param>
        /// <returns></returns>
        public async Task<FileEntityStorage> GetFileEntityStorageAsync(Guid storageProviderId, Guid? storageGroupId, Guid fileEntityId, List<StorageTypeEnum> storageTypes = null)
        {
            if (storageTypes == null)
            {
                storageTypes = Enum.GetValues<StorageTypeEnum>().Cast<StorageTypeEnum>().ToList();
            }

            var query = _dbContext.FileEntityStorage
                .Include(x => x.Storage)
                .ThenInclude(x => x.StorageGroup)
                .Include(x => x.FileEntity)
                .Where(x => x.Storage.StorageGroup.StorageProviderId == storageProviderId &&
                    x.FileEntityId == fileEntityId &&
                    storageTypes.Contains(x.Storage.Type) &&
                    x.Status == FileEntityStorageStatusEnum.None);

            if (storageGroupId != null)
            {
                query = query.Where(x => x.Storage.StorageGroupId == storageGroupId);

                var stroageGroup = (await _dbContext.StorageGroup.FirstAsync(x => x.StorageGroupId == storageGroupId));
                if (stroageGroup.Status.HasFlag(StorageGroupStatusEnum.Disable))
                {
                    throw new InvalidOperationException($"{ stroageGroup.Name } 無效");
                }
            }

            var fileStorage = (await query.ToListAsync())
                .Where(x => !x.Storage.Status.HasFlag(StorageStatusEnum.Disable))
                .OrderByDescending(x => x.Storage.StorageGroup.DownloadPriority) // 未強制指定時，先透過下載優先度排序
                .ThenBy(x => Guid.NewGuid()) // 下載優先度一樣時，亂數排序
                .FirstOrDefault();

            if (fileStorage == null)
            {
                throw new OperationFailedException(ResultCodeEnum.NotExistValidFileEntityStorage, "Could't find valid storage.");
            }
            return fileStorage;
        }

        /// <summary>
        /// 取得需同步的同步任務資訊
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="takeCount">取得數量上限</param>
        /// <param name="fileEntityNoDivisor">檔案實體流水號除數</param>
        /// <param name="fileEntityNoRemainder">檔案實體流水號餘數</param>
        /// <returns></returns>
        public async Task<List<SyncFileEntity>> GetSyncFileEntitiesAsync(Guid storageProviderId, int takeCount, int fileEntityNoDivisor, int fileEntityNoRemainder)
        {
            List<SyncFileEntity> result = new List<SyncFileEntity>();
            var storageProvider = await this.FindByIdAsync(storageProviderId);
            var sourceStorageGroups = storageProvider.StorageGroup.Where(x => !x.Status.HasFlag(StorageGroupStatusEnum.Disable)).ToList();
            var targetStorageGroups = storageProvider.StorageGroup.Where(x => x.SyncMode != SyncModeEnum.Never && !x.Status.HasFlag(StorageGroupStatusEnum.Disable)).ToList();

            for (int i = 0; i < sourceStorageGroups.Count; i++)
            {
                for (int j = 0; j < targetStorageGroups.Count; j++)
                {
                    var sourceStorageGroup = sourceStorageGroups[i];
                    var targetStorageGroup = targetStorageGroups[j];

                    if (sourceStorageGroup.StorageGroupId == targetStorageGroup.StorageGroupId)
                    {
                        continue;
                    }

                    var fileEntitiesQuery = _dbContext.FileEntity
                        .Include(x => x.FileEntityStroage)
                        .ThenInclude(x => x.Storage)
                        .ThenInclude(x => x.FileEntityStroage)
                        .Where(x => !x.IsMarkDelete)
                        .Where(x => x.FileEntityNo % fileEntityNoDivisor == fileEntityNoRemainder)
                        .Where(x => x.FileEntityStroage.Select(fs => fs.Storage.StorageGroup).Any(sg => sg.StorageGroupId == sourceStorageGroup.StorageGroupId) &&
                                    x.FileEntityStroage.Select(fs => fs.Storage.StorageGroup).All(sg => sg.StorageGroupId != targetStorageGroup.StorageGroupId))
                        .Where(x => x.FileEntityStroage.First(t => t.Storage.StorageGroupId == sourceStorageGroup.StorageGroupId).Status == FileEntityStorageStatusEnum.None) // 已經限制 Storage Group 了，所以 FileEntityStroage 必定唯一
                        .Where(x => !x.FileEntityStroage.First(t => t.Storage.StorageGroupId == sourceStorageGroup.StorageGroupId).IsMarkDelete)
                        .Where(x => !x.Status.HasFlag(FileEntityStatusEnum.Processing) && !x.Status.HasFlag(FileEntityStatusEnum.Uploading));

                    if (targetStorageGroup.SyncMode == SyncModeEnum.Policy)
                    {
                        fileEntitiesQuery = fileEntitiesQuery.ApplyPolicy(targetStorageGroup.SyncPolicy);
                    }

                    var fileEntities = await fileEntitiesQuery.Take(takeCount - result.Count).ToListAsync();
                    result.AddRange(fileEntities.Select(x => new SyncFileEntity()
                    {
                        FileEntity = x,
                        FromStorageGroup = sourceStorageGroup,
                        DestinationStorageGroup = targetStorageGroup,
                        FromFileEntityStorage = x.FileEntityStroage.First(t => t.Storage.StorageGroupId == sourceStorageGroup.StorageGroupId)
                    }));

                    if (result.Count >= takeCount)
                    {
                        return result;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 取得需執行刪除策略的檔案實體位於檔案檔案儲存群組的資訊
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="takeCount">取得數量上限</param>
        /// <param name="fileEntityNoDivisor">檔案實體流水號除數</param>
        /// <param name="fileEntityNoRemainder">檔案實體流水號餘數</param>
        /// <returns></returns>
        public async Task<List<FileEntityInStorageGroup>> GetClearFileEntityInStorageGroupAsync(Guid storageProviderId, int takeCount, int fileEntityNoDivisor, int fileEntityNoRemainder)
        {
            List<FileEntityInStorageGroup> result = new List<FileEntityInStorageGroup>();
            var storageProvider = await this.FindByIdAsync(storageProviderId);
            var storageGroups = storageProvider.StorageGroup.Where(x => !x.Status.HasFlag(StorageGroupStatusEnum.Disable) && x.ClearMode == ClearModeEnum.Start).ToList();

            for (int i = 0; i < storageGroups.Count; i++)
            {
                var fileEntityInStorageGroups = await _dbContext.FileEntity
                    .Where(x => x.FileEntityStroage.Any(x => x.Storage.StorageGroup.StorageGroupId == storageGroups[i].StorageGroupId) && !x.IsMarkDelete && x.Status == FileEntityStatusEnum.None && x.FileEntityNo % fileEntityNoDivisor == fileEntityNoRemainder)
                    .Select(x => new FileEntityInStorageGroup()
                    {
                        FileEntity = x,
                        FileEntityStorage = _dbContext.FileEntityStorage
                        .Include(t => t.Storage)
                        .ThenInclude(t => t.StorageGroup)
                        .FirstOrDefault(t => t.FileEntityId == x.FileEntityId && t.Storage.StorageGroupId == storageGroups[i].StorageGroupId),
                        ValidFileEntityStorageCount = _dbContext.FileEntityStorage.Count(t => t.FileEntityId == x.FileEntityId && t.Status == FileEntityStorageStatusEnum.None && !t.IsMarkDelete)
                    })
                    .ApplyPolicy(storageGroups[i].ClearPolicy)
                    .Take(takeCount - result.Count).ToListAsync();

                result.AddRange(fileEntityInStorageGroups);
                if (result.Count >= takeCount)
                {
                    return result;
                }
            }
            return result;
        }
        /// <summary>
        /// 取得儲存服務內不包含 Tag 且 MineType 為指定格式的檔案實體
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="tag">不包含的 Tag</param>
        /// <param name="mimeTypeParten">指定的 MineType 範本</param>
        /// <param name="isRootFileEntity">是否為跟檔案實體</param>
        /// <param name="validStorageTypes">驗證檔案實體是否擁有任一有效的檔案實體位於指定儲存類型的儲存個體上的資訊</param>
        /// <param name="takeCount">取得數量上限</param>
        /// <param name="fileEntityNoDivisor">檔案實體流水號除數</param>
        /// <param name="fileEntityNoRemainder">檔案實體流水號餘數</param>
        /// <returns></returns>
        public Task<List<FileEntity>> GetFileEntityWithoutTagAsync(Guid storageProviderId, string tag, string mimeTypeParten, bool isRootFileEntity, List<StorageTypeEnum> validStorageTypes, int takeCount, int fileEntityNoDivisor, int fileEntityNoRemainder)
        {
            var query = _dbContext.FileEntity.Where(x =>
                x.FileEntityNo % fileEntityNoDivisor == fileEntityNoRemainder &&
                x.FileEntityStroage.Any(f => f.Storage.StorageGroup.StorageProviderId == storageProviderId) &&
                x.FileEntityTag.All(t => t.Value != tag) &&
                !x.Status.HasFlag(FileEntityStatusEnum.Processing) &&
                !x.Status.HasFlag(FileEntityStatusEnum.Uploading) &&
                x.FileEntityStroage.Any(fes => validStorageTypes.Contains(fes.Storage.Type) && fes.Status == FileEntityStorageStatusEnum.None) &&
                EF.Functions.Like(x.MimeType, mimeTypeParten));

            if (isRootFileEntity)
            {
                query = query.Where(x => x.ParentFileEntityID == null);
            }

            return query.Take(takeCount).ToListAsync();
        }
        /// <summary>
        /// 取得儲存服務內包含 Tag 且 MineType 為指定格式的檔案實體
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="tag">不包含的 Tag</param>
        /// <param name="mimeTypeParten">指定的 MineType 範本</param>
        /// <param name="isRootFileEntity">是否為跟檔案實體</param>
        /// <param name="validStorageTypes">驗證檔案實體是否擁有任一有效的檔案實體位於指定儲存類型的儲存個體上的資訊</param>
        /// <param name="takeCount">取得數量上限</param>
        /// <param name="fileEntityNoDivisor">檔案實體流水號除數</param>
        /// <param name="fileEntityNoRemainder">檔案實體流水號餘數</param>
        /// <returns></returns>
        public Task<List<FileEntity>> GetFileEntityWithTagAsync(Guid storageProviderId, string tag, string mimeTypeParten, bool isRootFileEntity, List<StorageTypeEnum> validStorageTypes, int takeCount, int fileEntityNoDivisor, int fileEntityNoRemainder)
        {
            var query = _dbContext.FileEntity.Where(x =>
                x.FileEntityNo % fileEntityNoDivisor == fileEntityNoRemainder &&
                x.FileEntityStroage.Any(f => f.Storage.StorageGroup.StorageProviderId == storageProviderId) &&
                x.FileEntityTag.Any(t => t.Value == tag) &&
                !x.Status.HasFlag(FileEntityStatusEnum.Processing) &&
                !x.Status.HasFlag(FileEntityStatusEnum.Uploading) &&
                x.FileEntityStroage.Any(fes => validStorageTypes.Contains(fes.Storage.Type) && fes.Status == FileEntityStorageStatusEnum.None) &&
                EF.Functions.Like(x.MimeType, mimeTypeParten));

            if (isRootFileEntity)
            {
                query = query.Where(x => x.ParentFileEntityID == null);
            }

            return query.Take(takeCount).ToListAsync();
        }
        #endregion
    }
}
