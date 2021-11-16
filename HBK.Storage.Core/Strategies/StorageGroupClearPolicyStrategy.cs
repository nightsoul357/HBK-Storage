using HBK.Storage.Adapter.Models;
using HBK.Storage.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Strategies
{
    /// <summary>
    /// 儲存群組清除策略
    /// </summary>
    public static class StorageGroupClearPolicyStrategy
    {
        /// <summary>
        /// 附加策略篩選
        /// </summary>
        /// <param name="fileEntityInStorageGroups">檔案實體位於檔案檔案儲存群組的資訊清單</param>
        /// <param name="clearPolicy">同步策略</param>
        /// <returns></returns>
        public static IQueryable<FileEntityInStorageGroup> ApplyPolicy(this IQueryable<FileEntityInStorageGroup> fileEntityInStorageGroups, ClearPolicy clearPolicy)
        {
            if (!String.IsNullOrEmpty(clearPolicy.Rule))
            {
                fileEntityInStorageGroups = fileEntityInStorageGroups
                    .Where(clearPolicy.Rule);
            }

            return fileEntityInStorageGroups;
        }
    }
}
