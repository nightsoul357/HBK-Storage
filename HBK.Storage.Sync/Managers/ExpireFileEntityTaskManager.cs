using HBK.Storage.Core.Services;
using HBK.Storage.Sync.Extensions;
using HBK.Storage.Sync.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.Sync.Managers
{
    /// <summary>
    /// 過期檔案處理任務管理者
    /// </summary>
    public class ExpireFileEntityTaskManager : TaskManagerBase<ExpireFileEntityTaskManager, ExpireFileEntityTaskManagerOption>
    {

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public ExpireFileEntityTaskManager(ILogger<ExpireFileEntityTaskManager> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        protected override void StartInternal()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var taskId = Guid.NewGuid();
                using (var scope = _serviceProvider.CreateScope())
                {
                    var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                    var shouldMarkDeleteFileEntity = fileEntityService.GetExpireFileEntityWithoutMarkDeleteAsync(base.Option.FetchLimit, base.Option.FileEntityNoDivisor, base.Option.FileEntityNoRemainder).Result;
                    foreach (var fileEntity in shouldMarkDeleteFileEntity)
                    {
                        base.LogInformation(taskId, fileEntity, null, "已過期並標註為刪除");

                        fileEntityService.MarkFileEntityDeleteAsync(fileEntity.FileEntityId).Wait();
                    }
                    if (shouldMarkDeleteFileEntity.Count == 0)
                    {
                        SpinWait.SpinUntil(() => false, 1000);
                    }
                }
            }
        }
        public override void Dispose()
        {
        }
    }
}
