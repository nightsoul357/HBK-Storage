using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Models;
using HBK.Storage.Adapter.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace HBK.Storage.Core.Strategies
{
    /// <summary>
    /// 檔案實體同步策略
    /// </summary>
    public static class FileEntitySyncPolicyStrategy
    {
        /// <summary>
        /// 附加策略篩選
        /// </summary>
        /// <param name="fileEntities">檔案實體清單</param>
        /// <param name="syncPolicy">同步策略</param>
        /// <returns></returns>
        public static IQueryable<FileEntity> ApplyPolicy(this IQueryable<FileEntity> fileEntities, SyncPolicy syncPolicy)
        {
            if (!String.IsNullOrEmpty(syncPolicy.Rule))
            {
                fileEntities = fileEntities
                    .Where(syncPolicy.Rule);
            }

            return fileEntities;
        }
    }
}
