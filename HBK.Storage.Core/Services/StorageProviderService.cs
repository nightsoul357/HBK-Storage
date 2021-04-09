using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
                    .Where(x => !x.Status.HasFlag(StorageGroupStatus.Disable))
                    .Select(x =>
                        new
                        {
                            StorageGroup = x,
                            StoragExtendProperty = _storageGroupService.GetMaxRemainSizeStorageByStorageGroupIdAsync(x.StorageGroupId).Result
                        })
                    .OrderByDescending(x => x.StoragExtendProperty?.RemainSize)
                    .ToList();

                var sgMain = sg.Where(x => x.StorageGroup.Status.HasFlag(StorageGroupStatus.Main)).FirstOrDefault();

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

            if (storageGroup.Status.HasFlag(StorageGroupStatus.Disable))
            {
                throw new InvalidOperationException($"Storage Group { storageGroup.Name } is disabled.");
            }
            if (storageExtendProperty == null || storageExtendProperty.RemainSize <= fileEntity.Size)
            {
                throw new InvalidOperationException("Remain size not enough.");
            }
            #endregion
            Guid taskId = Guid.NewGuid();
            fileEntity.Status = FileEntityStatusEnum.Uploading | fileEntity.Status;
            fileEntity.FileEntityStroage.Add(new FileEntityStroage()
            {
                CreatorIdentity = "Upload Service",
                Status = FileEntityStorageStatusEnum.None,
                StorageId = storageExtendProperty.Storage.StorageId,
                Value = taskId.ToString()
            });
            _dbContext.FileEntity.Add(fileEntity);
            await _dbContext.SaveChangesAsync();

            var fileProvider = _fileSystemFactory.GetAsyncFileProvider(storageExtendProperty.Storage);
            var fileInfoResult = await fileProvider.PutAsync(taskId.ToString(), fileStream);
            fileEntity.Status = fileEntity.Status & ~FileEntityStatusEnum.Uploading;
            fileEntity.Size = fileInfoResult.Length;
            await _dbContext.SaveChangesAsync();
            return fileEntity;
        }

        /// <summary>
        /// 取得指定儲存服務內的指定檔案實體的檔案資訊
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="storageGroupId">強制指定儲存個體群組 ID</param>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <returns></returns>
        public async Task<IAsyncFileInfo> GetAsyncFileInfoAsync(Guid storageProviderId, Guid? storageGroupId, Guid fileEntityId)
        {
            var query = _dbContext.FileEntityStroage
                .Include(x => x.Storage)
                .Where(x => x.Storage.StorageGroup.StorageProviderId == storageProviderId && x.FileEntityId == fileEntityId);

            if (storageGroupId != null)
            {
                query = query.Where(x => x.Storage.StorageGroupId == storageGroupId);
            }

            var fileStorage = (await query.ToListAsync())
                .Where(x => !x.Storage.Status.HasFlag(StorageStatusEnum.Disable))
                .OrderBy(x => Guid.NewGuid()) // 未強制指定時，隨機取得 Storage
                .FirstOrDefault();

            if (fileStorage == null)
            {
                throw new InvalidOperationException("Could't find valud storage.");
            }

            var fileProvider = _fileSystemFactory.GetAsyncFileProvider(fileStorage.Storage);
            return await fileProvider.GetFileInfoAsync(fileStorage.Value);
        }
        #endregion
    }
}
