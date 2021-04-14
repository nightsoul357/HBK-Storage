using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
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
    public sealed class DeleteFileEntityTaskManager
    {
        private readonly ILogger<DeleteFileEntityTaskManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        private readonly DeleteFileEntityTaskManagerOption _option;

        private ConcurrentQueue<FileEntityStorage> _pendingQueue;
        private object _syncObj = new object();
        private CancellationToken _cancellationToken;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public DeleteFileEntityTaskManager(ILogger<DeleteFileEntityTaskManager> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceScope = _serviceProvider.CreateScope();

            _option = _serviceScope.ServiceProvider.GetRequiredService<DeleteFileEntityTaskManagerOption>();
        }
        public void Start(CancellationToken cancellationToken)
        {
            if (!this.IsRunning)
            {
                lock (_syncObj)
                {
                    if (!this.IsRunning)
                    {
                        _cancellationToken = cancellationToken;
                        _cancellationToken.Register(this.Cancel);
                        _pendingQueue = new ConcurrentQueue<FileEntityStorage>();
                        Task.Factory.StartNew(this.StartInternal, TaskCreationOptions.LongRunning);
                        this.IsRunning = true;
                    }
                }
            }
        }
        private void StartInternal()
        {
            List<Task> tasks = new List<Task>();
            using var scope = _serviceProvider.CreateScope();
            var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();

            while (!_cancellationToken.IsCancellationRequested)
            {
                this.FetchTask();

                if (_pendingQueue.Count != 0)
                {
                    for (int i = 0; i < _option.TaskLimit; i++)
                    {
                        tasks.Add(Task.Factory.StartNew(this.ExecuteTask, TaskCreationOptions.LongRunning));
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
            using (var scope = _serviceProvider.CreateScope())
            {
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                var fileEntityStroagies = fileEntityService
                    .GetMarkDeleteFileEntityStoragiesAsync(_option.FetchLimit, _option.FileEntityNoDivisor, _option.FileEntityNoRemainder).Result;
                fileEntityStroagies.ForEach(x => _pendingQueue.Enqueue(x));
            }
        }
        private void ExecuteTask()
        {
            while (!_cancellationToken.IsCancellationRequested && _pendingQueue.TryDequeue(out FileEntityStorage fileEntityStroage))
            {
                Guid deleteTaskId = Guid.NewGuid();
                using (var scope = _serviceProvider.CreateScope())
                {
                    var fileSystemFactory = scope.ServiceProvider.GetRequiredService<FileSystemFactory>();
                    var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();
                    var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();
                    try
                    {
                        _logger.LogInformation(_option.Identity, "刪除開始", deleteTaskId, "正在將檔案 ID 為 {0} 的 {1} 從 {2} 群組中的 {3} 儲存個體中刪除",
                        fileEntityStroage.FileEntityId,
                        fileEntityStroage.FileEntity.Name,
                        fileEntityStroage.Storage.StorageGroup.Name,
                        fileEntityStroage.Storage.Name);

                        if (!fileEntityStorageService.ValidateFileEntityStorageAsync(fileEntityStroage.FileEntityStorageId).Result)
                        {
                            _logger.LogInformation(_option.Identity, "刪除完成", deleteTaskId, "檔案 ID 為 {0} 的 {1} 在 {2} 群組中的 {3} 儲存個體中無法找到，故直接刪除其相關資訊",
                            fileEntityStroage.FileEntityId,
                            fileEntityStroage.FileEntity.Name,
                            fileEntityStroage.Storage.StorageGroup.Name,
                            fileEntityStroage.Storage.Name);
                            continue;
                        }

                        var fileProvider = fileSystemFactory.GetAsyncFileProvider(fileEntityStroage.Storage);
                        fileProvider.DeleteAsync(fileEntityStroage.Value).Wait();
                        fileEntityService.DeleteFileEntityStroageAsync(fileEntityStroage.FileEntityStorageId).Wait();

                        _logger.LogInformation(_option.Identity, "刪除完成", deleteTaskId, "檔案 ID 為 {0} 的 {1} 從 {2} 檔案群組中的 {3} 檔案儲存個體中刪除",
                        fileEntityStroage.FileEntityId,
                        fileEntityStroage.FileEntity.Name,
                        fileEntityStroage.Storage.StorageGroup.Name,
                        fileEntityStroage.Storage.Name);
                    }
                    catch (Exception ex)
                    {
                        LoggerExtensions.LogError(_logger, _option.Identity, "刪除失敗", deleteTaskId, ex, "發生未預期的例外");
                    }
                }
            }
        }
        private void Cancel()
        {
            _serviceScope.Dispose();
        }

        public bool IsRunning { get; set; }
    }
}
