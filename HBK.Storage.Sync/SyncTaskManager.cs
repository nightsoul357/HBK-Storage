using Amazon.Runtime.Internal.Util;
using HBK.Storage.Adapter.Enums;
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

namespace HBK.Storage.Sync
{
    /// <summary>
    /// 同步任務管理者
    /// </summary>
    public sealed class SyncTaskManager
    {

        private readonly ILogger<SyncTaskManager> _logger;
        private readonly SyncTaskManagerOption _option;
        private readonly StorageProviderService _storageProviderService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        private readonly FileSystemFactory _fileSystemFactory;
        private readonly StorageGroupService _storageGroupService;
        private readonly StorageService _storageService;

        private Task _internalTask;
        private ConcurrentQueue<SyncTaskModel> _pendingQueue;
        private CancellationToken _cancellationToken;
        private object _syncObj = new object();

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="option"></param>
        /// <param name="storageProviderService"></param>
        public SyncTaskManager(ILogger<SyncTaskManager> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceScope = _serviceProvider.CreateScope();
            _option = _serviceScope.ServiceProvider.GetRequiredService<SyncTaskManagerOption>();
            _fileSystemFactory = _serviceScope.ServiceProvider.GetRequiredService<FileSystemFactory>();
            _storageProviderService = _serviceScope.ServiceProvider.GetRequiredService<StorageProviderService>();
            _storageGroupService = _serviceScope.ServiceProvider.GetRequiredService<StorageGroupService>();
            _storageService = _serviceScope.ServiceProvider.GetRequiredService<StorageService>();
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
                        _pendingQueue = new ConcurrentQueue<SyncTaskModel>();
                        _internalTask = Task.Factory.StartNew(this.StartInternal, TaskCreationOptions.LongRunning);
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

                for (int i = 0; i < _option.TaskLimit; i++)
                {
                    tasks.Add(Task.Factory.StartNew(this.ExecuteTask, TaskCreationOptions.LongRunning));
                }

                Task.WaitAll(tasks.ToArray());
            }
        }

        private void FetchTask()
        {
            List<StorageProvider> storageProviders = new List<StorageProvider>();

            if (_option.IsExecutOnAllStoragProvider)
            {
                storageProviders.AddRange(_storageProviderService.GetAllStorageProviderAsync().Result);
            }
            else
            {
                storageProviders.AddRange(_storageProviderService.GetStorageProviderByIdsAsync(_option.StorageProviderIds).Result);
            }

            foreach (var storageProvider in storageProviders)
            {
                var tasks = _storageProviderService.GetSyncFileEntitiesAsync(storageProvider.StorageProviderId,
                    _option.FetchLimit - _pendingQueue.Count,
                    _option.FileEntityNoDivisor,
                    _option.FileEntityNoRemainder).Result;

                tasks.ForEach(x => _pendingQueue.Enqueue(new SyncTaskModel()
                {
                    CreateDateTime = DateTimeOffset.Now,
                    DestinationStorageGroup = x.DestinationStorageGroup,
                    FileEntity = x.FileEntity,
                    FromFileEntityStorage = x.FromFileEntityStorage,
                    FromStorageGroup = x.FromStorageGroup
                }));

                if(_pendingQueue.Count >= _option.FetchLimit)
                {
                    break;
                }
            }
        }

        private void ExecuteTask()
        {
            while (_pendingQueue.TryDequeue(out SyncTaskModel syncTaskModel))
            {
                var desStorage = _storageGroupService.GetMaxRemainSizeStorageByStorageGroupIdAsync(syncTaskModel.DestinationStorageGroup.StorageGroupId).Result;
                if (desStorage.RemainSize < syncTaskModel.FileEntity.Size)
                {
                    _ = _storageService.AddFileEntityInStorageAsync(new FileEntityStroage()
                    {
                        CreatorIdentity = _option.Identity,
                        FileEntityId = syncTaskModel.FileEntity.FileEntityId,
                        Status = FileEntityStorageStatusEnum.SyncFail,
                        StorageId = desStorage.Storage.StorageId,
                        Value = "Sync Fail - Remain space not enough"
                    }).Result;
                    continue;
                }

                var fromProvider = _fileSystemFactory.GetAsyncFileProvider(syncTaskModel.FromFileEntityStorage.Storage);
                var desProvider = _fileSystemFactory.GetAsyncFileProvider(desStorage.Storage);
                var fileInfo = fromProvider.GetFileInfo(syncTaskModel.FromFileEntityStorage.Value);
                Guid taskId = Guid.NewGuid();

                var desFileEntityStorage = _storageService.AddFileEntityInStorageAsync(new FileEntityStroage()
                {
                    CreatorIdentity = _option.Identity,
                    FileEntityId = syncTaskModel.FileEntity.FileEntityId,
                    Status = FileEntityStorageStatusEnum.Syncing,
                    StorageId = desStorage.Storage.StorageId,
                    Value = taskId.ToString()
                }).Result;

                var desFileIno = desProvider.PutAsync(desFileEntityStorage.Value, fileInfo.CreateReadStream()).Result;
                _storageService.CompleteSyncAsync(desFileEntityStorage.FileEntityStroageId).Wait();
            }
        }

        private void Cancel()
        {
            int r = 10;
        }
        public bool IsRunning { get; set; }
    }
}
