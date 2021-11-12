using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.Models;
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
    public class ClearTaskManager : TaskManagerBase<ClearTaskManager, ClearTaskManagerOption>
    {
        private ConcurrentQueue<FileEntityInStorageGroup> _pendingQueue;

        public ClearTaskManager(ILogger<ClearTaskManager> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }
        protected override void StartInternal()
        {
            _pendingQueue = new ConcurrentQueue<FileEntityInStorageGroup>();
            List<Task> tasks = new List<Task>();
            while (!_cancellationToken.IsCancellationRequested)
            {
                this.FetchTask();

                if (_pendingQueue.Count != 0)
                {
                    for (int i = 0; i < base.Option.TaskLimit; i++)
                    {
                        tasks.Add(Task.Factory.StartNewSafety(this.ExecuteTask, TaskCreationOptions.LongRunning, base.ExcetpionHandle));
                    }

                    Task.WaitAll(tasks.ToArray());
                }
                else
                {
                    SpinWait.SpinUntil(() => false, 1000);
                }
            }
        }
        private void FetchTask()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                List<StorageProvider> storageProviders = new List<StorageProvider>();

                if (base.Option.IsExecutOnAllStoragProvider)
                {
                    storageProviders.AddRange(storageProviderService.GetAllStorageProviderAsync().Result);
                }
                else
                {
                    storageProviders.AddRange(storageProviderService.GetStorageProviderByIdsAsync(base.Option.StorageProviderIds).Result);
                }

                foreach (var storageProvider in storageProviders)
                {
                    var fileEntityInStorageGroups = storageProviderService.GetClearFileEntityInStorageGroupAsync(storageProvider.StorageProviderId,
                        base.Option.FetchLimit - _pendingQueue.Count,
                        base.Option.FileEntityNoDivisor,
                        base.Option.FileEntityNoRemainder).Result;

                    fileEntityInStorageGroups.ForEach(x => _pendingQueue.Enqueue(x));
                }
            }
        }
        private void ExecuteTask()
        {
            while (!_cancellationToken.IsCancellationRequested && _pendingQueue.TryDequeue(out FileEntityInStorageGroup fileEntityInStorageGroup))
            {
                var taskId = Guid.NewGuid();
                using (var scope = _serviceProvider.CreateScope())
                {
                    var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                    fileEntityStorageService.MarkFileEntityStorageDeleteAsync(fileEntityInStorageGroup.FileEntityStorage.FileEntityStorageId).Wait();
                    base.LogInformation(taskId, fileEntityInStorageGroup.FileEntity, null, $"根據刪除策略將位於 {fileEntityInStorageGroup.FileEntityStorage.Storage.StorageGroup.Name} 上的檔案標記為刪除");
                }
            }
        }
        public override void Dispose()
        {

        }
    }
}
