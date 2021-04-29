using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.Sync.Extensions;
using HBK.Storage.Sync.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.Sync.Managers
{
    /// <summary>
    /// 刪除檔案實體任務管理者
    /// </summary>
    public sealed class DeleteFileEntityTaskManager : TaskManagerBase<DeleteFileEntityTaskManager>
    {
        private readonly IServiceScope _serviceScope;
        private readonly DeleteFileEntityTaskManagerOption _option;

        private ConcurrentQueue<FileEntityStorage> _pendingQueue;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public DeleteFileEntityTaskManager(ILogger<DeleteFileEntityTaskManager> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
            _serviceScope = _serviceProvider.CreateScope();
            _option = _serviceScope.ServiceProvider.GetRequiredService<DeleteFileEntityTaskManagerOption>();
        }
        protected override void StartInternal()
        {
            List<Task> tasks = new List<Task>();
            using var scope = _serviceProvider.CreateScope();
            var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
            _pendingQueue = new ConcurrentQueue<FileEntityStorage>();

            while (!base._cancellationToken.IsCancellationRequested)
            {
                this.FetchTask();

                if (_pendingQueue.Count != 0)
                {
                    for (int i = 0; i < _option.TaskLimit; i++)
                    {
                        tasks.Add(Task.Factory.StartNewSafety(this.ExecuteTask, TaskCreationOptions.LongRunning, base.ExcetpionHandle));
                    }

                    Task.WaitAll(tasks.ToArray());
                }
                else
                {
                    SpinWait.SpinUntil(() => false, 1000);
                }

                _ = fileEntityService.UpdateFileEntityDeleteInfoAsync(_option.FetchLimit, _option.FileEntityNoDivisor, _option.FileEntityNoRemainder).Result;
            }
        }
        private void FetchTask()
        {
            using (var scope = base._serviceProvider.CreateScope())
            {
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                var fileEntityStroagies = fileEntityService
                    .GetMarkDeleteFileEntityStoragiesAsync(_option.FetchLimit, _option.FileEntityNoDivisor, _option.FileEntityNoRemainder).Result;
                fileEntityStroagies.ForEach(x => _pendingQueue.Enqueue(x));
            }
        }
        private void ExecuteTask()
        {
            while (!base._cancellationToken.IsCancellationRequested && _pendingQueue.TryDequeue(out FileEntityStorage fileEntityStroage))
            {
                Guid deleteTaskId = Guid.NewGuid();
                using (var scope = _serviceProvider.CreateScope())
                {
                    var fileSystemFactory = scope.ServiceProvider.GetRequiredService<FileSystemFactory>();
                    var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                    var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                    try
                    {
                        base._logger.LogCustomInformation(_option.Identity, "刪除開始", deleteTaskId, "正在將檔案 ID 為 {0} 的 {1} 從 {2} 群組中的 {3} 儲存個體中刪除",
                        fileEntityStroage.FileEntityId,
                        fileEntityStroage.FileEntity.Name,
                        fileEntityStroage.Storage.StorageGroup.Name,
                        fileEntityStroage.Storage.Name);

                        if (fileEntityStorageService.TryFetchFileInfoAsync(fileEntityStroage.FileEntityStorageId).Result == null)
                        {
                            base._logger.LogCustomInformation(_option.Identity, "刪除完成", deleteTaskId, "檔案 ID 為 {0} 的 {1} 在 {2} 群組中的 {3} 儲存個體中無法找到，故直接刪除其相關資訊",
                            fileEntityStroage.FileEntityId,
                            fileEntityStroage.FileEntity.Name,
                            fileEntityStroage.Storage.StorageGroup.Name,
                            fileEntityStroage.Storage.Name);
                            fileEntityService.DeleteFileEntityStroageAsync(fileEntityStroage.FileEntityStorageId).Wait();
                            continue;
                        }

                        var fileProvider = fileSystemFactory.GetAsyncFileProvider(fileEntityStroage.Storage);
                        fileProvider.DeleteAsync(fileEntityStroage.Value).Wait();
                        fileEntityService.DeleteFileEntityStroageAsync(fileEntityStroage.FileEntityStorageId).Wait();

                        base._logger.LogCustomInformation(_option.Identity, "刪除完成", deleteTaskId, "檔案 ID 為 {0} 的 {1} 從 {2} 檔案群組中的 {3} 檔案儲存個體中刪除",
                        fileEntityStroage.FileEntityId,
                        fileEntityStroage.FileEntity.Name,
                        fileEntityStroage.Storage.StorageGroup.Name,
                        fileEntityStroage.Storage.Name);
                    }
                    catch (Exception ex)
                    {
                        base._logger.LogCustomError(_option.Identity, "刪除失敗", deleteTaskId, ex, "發生未預期的例外");
                    }
                }
            }
        }
        public override void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
