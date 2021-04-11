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
        private readonly FileEntityService _fileEntityService;
        private readonly FileSystemFactory _fileSystemFactory;

        private ConcurrentQueue<FileEntityStroage> _pendingQueue;
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
            _fileEntityService = _serviceScope.ServiceProvider.GetRequiredService<FileEntityService>();
            _fileSystemFactory = _serviceScope.ServiceProvider.GetRequiredService<FileSystemFactory>();
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
                        _pendingQueue = new ConcurrentQueue<FileEntityStroage>();
                        Task.Factory.StartNew(this.StartInternal, TaskCreationOptions.LongRunning);
                        this.IsRunning = true;
                    }
                }
            }
        }
        private void StartInternal()
        {
            List<Task> tasks = new List<Task>();
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
                    SpinWait.SpinUntil(() => true, 1000);
                }

                _ = _fileEntityService.UpdateFileEntityDeleteInfoAsync(_option.FetchLimit, _option.FileEntityNoDivisor, _option.FileEntityNoRemainder).Result;
            }
        }
        private void FetchTask()
        {
            var fileEntityStroagies = _fileEntityService
                .GetMarkDeleteFileEntityStoragiesAsync(_option.FetchLimit, _option.FileEntityNoDivisor, _option.FileEntityNoRemainder).Result;
            fileEntityStroagies.ForEach(x => _pendingQueue.Enqueue(x));
        }
        private void ExecuteTask()
        {
            while (!_cancellationToken.IsCancellationRequested && _pendingQueue.TryDequeue(out FileEntityStroage fileEntityStroage))
            {
                try
                {
                    _logger.LogInformation("[正在刪除] 正在將檔案 ID 為 {0} 的 {1} 從 {2} 檔案群組中的 {3} 檔案儲存個體中刪除",
                        fileEntityStroage.FileEntityId,
                        fileEntityStroage.FileEntity.Name,
                        fileEntityStroage.Storage.StorageGroup.Name,
                        fileEntityStroage.Storage.Name);
                    var fileProvider = _fileSystemFactory.GetAsyncFileProvider(fileEntityStroage.Storage);
                    fileProvider.DeleteAsync(fileEntityStroage.Value).Wait();
                    _fileEntityService.DeleteFileEntityStroageAsync(fileEntityStroage.FileEntityStroageId).Wait();
                    _logger.LogInformation("[刪除完成] 檔案 ID 為 {0} 的 {1} 從 {2} 檔案群組中的 {3} 檔案儲存個體中刪除 作業完成",
                        fileEntityStroage.FileEntityId,
                        fileEntityStroage.FileEntity.Name,
                        fileEntityStroage.Storage.StorageGroup.Name,
                        fileEntityStroage.Storage.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Uncatch Error");
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
