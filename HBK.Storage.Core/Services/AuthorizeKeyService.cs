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
            return _dbContext.AuthorizeKey;
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
        #endregion
    }
}
