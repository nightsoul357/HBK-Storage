using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Services
{
    /// <summary>
    /// 驗證金鑰服務
    /// </summary>
    public class AuthorizeKeyService
    {
        private readonly HBKStorageContext _dbContext;

        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="dbContext">資料庫實體</param>
        public AuthorizeKeyService(HBKStorageContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region DAL
        /// <summary>
        /// 取得金鑰列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<AuthorizeKey> ListQuery()
        {
            return _dbContext.AuthorizeKey
                .Include(x => x.AuthorizeKeyScope);
        }
        /// <summary>
        /// 以 ID 取得金鑰
        /// </summary>
        /// <param name="authorizeKeyId">金鑰 ID</param>
        /// <returns></returns>
        public Task<AuthorizeKey> FindByIdAsync(Guid authorizeKeyId)
        {
            return this.ListQuery()
                .FirstOrDefaultAsync(x => x.AuthorizeKeyId == authorizeKeyId);
        }
        /// <summary>
        /// 以金鑰值取得金鑰
        /// </summary>
        /// <param name="keyValue">金鑰值</param>
        /// <returns></returns>
        public Task<AuthorizeKey> FindByKeyValueAsync(string keyValue)
        {
            return this.ListQuery()
                .FirstOrDefaultAsync(x => x.KeyValue == keyValue);
        }
        /// <summary>
        /// 新增金鑰
        /// </summary>
        /// <param name="authorizeKey">新增的金鑰</param>
        /// <returns></returns>
        public async Task<AuthorizeKey> AddAsync(AuthorizeKey authorizeKey)
        {
            if (authorizeKey.AuthorizeKeyId == default)
            {
                authorizeKey.AuthorizeKeyId = Guid.NewGuid();
            }

            if (String.IsNullOrEmpty(authorizeKey.KeyValue))
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    var data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(authorizeKey.AuthorizeKeyId.ToString() + StringHelper.GetRandomString(10)));
                    var key = Encoding.UTF8.GetString(data);
                    authorizeKey.KeyValue = authorizeKey.Name + "_" + key;
                }
            }

            _dbContext.AuthorizeKey.Add(authorizeKey);
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(authorizeKey.AuthorizeKeyId);
        }
        /// <summary>
        /// 更新驗證金鑰
        /// </summary>
        /// <param name="authorizeKey"></param>
        /// <returns></returns>
        public async Task<AuthorizeKey> UpdateAsync(AuthorizeKey authorizeKey)
        {
            AuthorizeKey original = await this.FindByIdAsync(authorizeKey.AuthorizeKeyId);
            original.ExtendProperty = authorizeKey.ExtendProperty;
            original.KeyValue = authorizeKey.KeyValue;
            original.Name = authorizeKey.Name;
            original.Status = authorizeKey.Status;
            original.Type = authorizeKey.Type;
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(authorizeKey.AuthorizeKeyId);
        }
        /// <summary>
        /// 移除驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Guid authorizeKeyId)
        {
            _dbContext.AuthorizeKey.Remove(await _dbContext.AuthorizeKey.FirstOrDefaultAsync(x => x.AuthorizeKeyId == authorizeKeyId));
            await _dbContext.SaveChangesAsync();
            return true;
        }
        #endregion
        #region BAL
        /// <summary>
        /// 取得指金鑰 ID、儲存服務 ID、允許的操作類型所對應的金鑰使用範圍資訊是否存在
        /// </summary>
        /// <param name="authorizeKeyId">金鑰 ID</param>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="authorizeKeyScopeOperationType">允許的操作類型</param>
        /// <returns></returns>
        public Task<bool> IsExistAuthorizeKeyScopeByStorageProviderAsync(Guid authorizeKeyId, Guid storageProviderId, AuthorizeKeyScopeOperationTypeEnum authorizeKeyScopeOperationType)
        {
            return _dbContext.AuthorizeKeyScope
                .AnyAsync(x => x.AuthorizeKeyId == authorizeKeyId &&
                    x.StorageProviderId == storageProviderId &&
                    x.AllowOperationType == authorizeKeyScopeOperationType);
        }
        /// <summary>
        /// 取得指金鑰 ID、儲存個體群組 ID、允許的操作類型所對應的金鑰使用範圍資訊是否存在
        /// </summary>
        /// <param name="authorizeKeyId">金鑰 ID</param>
        /// <param name="storageGroupId">儲存個體群組 ID</param>
        /// <param name="authorizeKeyScopeOperationType">允許的操作類型</param>
        /// <returns></returns>
        public async Task<bool> IsExistAuthorizeKeyScopeByStorageGroupAsync(Guid authorizeKeyId, Guid storageGroupId, AuthorizeKeyScopeOperationTypeEnum authorizeKeyScopeOperationType)
        {
            var storageProviderId = await _dbContext.StorageGroup
                .Where(x => x.StorageGroupId == storageGroupId)
                .Select(x => x.StorageProviderId)
                .FirstOrDefaultAsync();

            return storageProviderId == default ? false : await this.IsExistAuthorizeKeyScopeByStorageProviderAsync(authorizeKeyId, storageProviderId.Value, authorizeKeyScopeOperationType);
        }
        /// <summary>
        /// 取得指金鑰 ID、儲存個體 ID、允許的操作類型所對應的金鑰使用範圍資訊是否存在
        /// </summary>
        /// <param name="authorizeKeyId">金鑰 ID</param>
        /// <param name="storageId">儲存個體 ID</param>
        /// <param name="authorizeKeyScopeOperationType">允許的操作類型</param>
        /// <returns></returns>
        public async Task<bool> IsExistAuthorizeKeyScopeByStorageAsync(Guid authorizeKeyId, Guid storageId, AuthorizeKeyScopeOperationTypeEnum authorizeKeyScopeOperationType)
        {
            var storageProviderId = await _dbContext.Storage
                .Where(x => x.StorageId == storageId)
                .Select(x => x.StorageGroup.StorageProviderId)
                .FirstOrDefaultAsync();

            return storageProviderId == default ? false : await this.IsExistAuthorizeKeyScopeByStorageProviderAsync(authorizeKeyId, storageProviderId.Value, authorizeKeyScopeOperationType);
        }
        /// <summary>
        /// 取得指金鑰 ID、檔案實體 ID、允許的操作類型所對應的金鑰使用範圍資訊是否存在
        /// </summary>
        /// <param name="authorizeKeyId">金鑰 ID</param>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="authorizeKeyScopeOperationType">允許的操作類型</param>
        /// <returns></returns>
        public async Task<bool> IsExistAuthorizeKeyScopeByFileEntityAsync(Guid authorizeKeyId, Guid fileEntityId, AuthorizeKeyScopeOperationTypeEnum authorizeKeyScopeOperationType)
        {
            var storageProviderId = await _dbContext.FileEntity
                .Where(x => x.FileEntityId == fileEntityId)
                .Select(x => x.FileEntityStroage.First().Storage.StorageGroup.StorageProviderId)
                .FirstOrDefaultAsync();

            return storageProviderId == default ? false : await this.IsExistAuthorizeKeyScopeByStorageProviderAsync(authorizeKeyId, storageProviderId.Value, authorizeKeyScopeOperationType);
        }
        /// <summary>
        /// 取得指金鑰 ID、檔案存取權杖 ID、允許的操作類型所對應的金鑰使用範圍資訊是否存在
        /// </summary>
        /// <param name="authorizeKeyId">金鑰 ID</param>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <param name="authorizeKeyScopeOperationType">允許的操作類型</param>
        /// <returns></returns>
        public async Task<bool> IsExistAuthorizeKeyScopeByFileAccessTokenAsync(Guid authorizeKeyId, Guid fileAccessTokenId, AuthorizeKeyScopeOperationTypeEnum authorizeKeyScopeOperationType)
        {
            var storageProviderId = await _dbContext.FileAccessToken
                .Where(x => x.FileAccessTokenId == fileAccessTokenId)
                .Select(x => x.StorageProviderId)
                .FirstOrDefaultAsync();

            return storageProviderId == default ? false : await this.IsExistAuthorizeKeyScopeByStorageProviderAsync(authorizeKeyId, storageProviderId, authorizeKeyScopeOperationType);
        }
        /// <summary>
        /// 停用驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns></returns>
        public async Task<AuthorizeKey> DiableAsync(Guid authorizeKeyId)
        {
            AuthorizeKey authorizeKey = await this.FindByIdAsync(authorizeKeyId); ;
            authorizeKey.Status |= AuthorizeKeyStatusEnum.Disable; // 加入 Disable
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(authorizeKeyId);
        }
        /// <summary>
        /// 啟用驗證金鑰
        /// </summary>
        /// <param name="authorizeKeyId">驗證金鑰 ID</param>
        /// <returns></returns>
        public async Task<AuthorizeKey> EnableAsync(Guid authorizeKeyId)
        {
            AuthorizeKey authorizeKey = await this.FindByIdAsync(authorizeKeyId); ;
            authorizeKey.Status &= (~AuthorizeKeyStatusEnum.Disable); // 移除 Disable
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(authorizeKeyId);
        }
        #endregion
    }
}
