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
            return _dbContext.FileEntity;
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
        #endregion
    }
}
