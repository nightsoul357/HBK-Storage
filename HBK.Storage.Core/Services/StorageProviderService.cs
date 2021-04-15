using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Models;
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
        /// <returns></returns>
        public async Task<FileEntity> UploadFileEntityAsync(Guid storageProviderId, Guid? storageGroupId, FileEntity fileEntity, Stream fileStream)
        {
            var storageProvider = await this.FindByIdAsync(storageProviderId);
            StorageGroup storageGroup = null;
            StorageExtendProperty storageExtendProperty = null;

            #region 取得對應 Storage Group
            if (storageGroupId != null)
            {
                storageGroup = storageProvider.StorageGroup.FirstOrDefault(x => x.StorageGroupId == storageGroupId.Value);
                storageExtendProperty = await _storageGroupService.GetMaxRemainSizeStorageByStorageGroupIdAsync(storageGroup.StorageGroupId);
            }
            else if (storageGroupId == null)
            {
                var sg = storageProvider.StorageGroup
                    .Where(x => !x.Status.HasFlag(StorageGroupStatusEnum.Disable))
                    .Select(x =>
                        new
                        {
                            StorageGroup = x,
                            StoragExtendProperty = _storageGroupService.GetMaxRemainSizeStorageByStorageGroupIdAsync(x.StorageGroupId).Result
                        })
                    .OrderByDescending(x => x.StoragExtendProperty?.RemainSize)
                    .ToList();

                var sgMain = sg.Where(x => x.StorageGroup.Status.HasFlag(StorageGroupStatusEnum.Main)).FirstOrDefault();

                if (sgMain != null && sgMain.StoragExtendProperty?.RemainSize > fileEntity.Size)
                {
                    storageGroup = sgMain.StorageGroup;
                    storageExtendProperty = sgMain.StoragExtendProperty;
                }
                else
                {
                    var sgOther = sg.FirstOrDefault();
                    storageGroup = sgOther.StorageGroup;
                    storageExtendProperty = sgOther.StoragExtendProperty;
                }
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
                CreatorIdentity = "Upload Service",
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
        /// <returns></returns>
        public async Task<FileEntityStorage> GetFileEntityStorageAsync(Guid storageProviderId, Guid? storageGroupId, Guid fileEntityId)
        {
            var query = _dbContext.FileEntityStorage
                .Include(x => x.Storage)
                .Include(x => x.FileEntity)
                .Where(x => x.Storage.StorageGroup.StorageProviderId == storageProviderId && x.FileEntityId == fileEntityId);

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
                .OrderBy(x => Guid.NewGuid()) // 未強制指定時，隨機取得 Storage
                .FirstOrDefault();

            if (fileStorage == null)
            {
                throw new InvalidOperationException("Could't find valid storage.");
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
            var storageGroups = storageProvider.StorageGroup.Where(x => x.SyncMode != SyncModeEnum.Never && !x.Status.HasFlag(StorageGroupStatusEnum.Disable)).ToList();

            for (int i = 0; i < storageGroups.Count; i++)
            {
                for (int j = 0; j < storageGroups.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var sourceStorageGroup = storageGroups[i];
                    var targetStorageGroup = storageGroups[j];

                    var fileEntitiesQuery = _dbContext.FileEntity
                        .Include(x => x.FileEntityStroage)
                        .ThenInclude(x => x.Storage)
                        .ThenInclude(x => x.FileEntityStroage)
                        .Where(x => !x.IsMarkDelete)
                        .Where(x => x.FileEntityNo % fileEntityNoDivisor == fileEntityNoRemainder)
                        .Where(x => x.FileEntityStroage.Select(fs => fs.Storage.StorageGroup).Any(sg => sg.StorageGroupId == sourceStorageGroup.StorageGroupId) &&
                                    x.FileEntityStroage.Select(fs => fs.Storage.StorageGroup).All(sg => sg.StorageGroupId != targetStorageGroup.StorageGroupId))
                        .Where(x => x.FileEntityStroage.First(t => t.Storage.StorageGroupId == sourceStorageGroup.StorageGroupId).Status == FileEntityStorageStatusEnum.None) // 已經限制 Storage Group 了，所以 FileEntityStroage 必定唯一
                        .Where(x => !x.FileEntityStroage.First(t => t.Storage.StorageGroupId == sourceStorageGroup.StorageGroupId).IsMarkDelete);

                    if (targetStorageGroup.SyncMode == SyncModeEnum.Policy)
                    {
                        if (targetStorageGroup.SyncPolicy.TagMatchMode == TagMatchModeEnum.All)
                        {
                            fileEntitiesQuery = fileEntitiesQuery
                                .Where(f => f.FileEntityTag.All(st => EF.Functions.Like(st.Value, targetStorageGroup.SyncPolicy.TagRule)));
                        }
                        else
                        {
                            fileEntitiesQuery = fileEntitiesQuery
                                .Where(f => f.FileEntityTag.Any(st => EF.Functions.Like(st.Value, targetStorageGroup.SyncPolicy.TagRule)));
                        }
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
        #endregion
    }
}
