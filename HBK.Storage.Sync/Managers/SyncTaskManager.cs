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

namespace HBK.Storage.Sync.Managers
{
    /// <summary>
    /// 同步任務管理者
    /// </summary>
    public sealed class SyncTaskManager
    {
        private readonly ILogger<SyncTaskManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        private readonly SyncTaskManagerOption _option;

        private ConcurrentQueue<SyncTaskModel> _pendingQueue;
        private CancellationToken _cancellationToken;
        private object _syncObj = new object();

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public SyncTaskManager(ILogger<SyncTaskManager> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceScope = _serviceProvider.CreateScope();

            _option = _serviceScope.ServiceProvider.GetRequiredService<SyncTaskManagerOption>();
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

                if (_option.IsExecutOnAllStoragProvider)
                {
                    storageProviders.AddRange(storageProviderService.GetAllStorageProviderAsync().Result);
                }
                else
                {
                    storageProviders.AddRange(storageProviderService.GetStorageProviderByIdsAsync(_option.StorageProviderIds).Result);
                }

                foreach (var storageProvider in storageProviders)
                {
                    var tasks = storageProviderService.GetSyncFileEntitiesAsync(storageProvider.StorageProviderId,
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

                    if (_pendingQueue.Count >= _option.FetchLimit)
                    {
                        break;
                    }
                }
            }
        }

        private void ExecuteTask()
        {
            while (!_cancellationToken.IsCancellationRequested && _pendingQueue.TryDequeue(out SyncTaskModel syncTaskModel))
            {
                Guid sysncTaskIdentity = Guid.NewGuid();
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                        var storageGroupService = scope.ServiceProvider.GetRequiredService<StorageGroupService>();
                        var storgaeService = scope.ServiceProvider.GetRequiredService<StorageService>();
                        var fileSystemFacotry = scope.ServiceProvider.GetRequiredService<FileSystemFactory>();

                        var desStorage = storageGroupService.GetMaxRemainSizeStorageByStorageGroupIdAsync(syncTaskModel.DestinationStorageGroup.StorageGroupId).Result;
                        if (desStorage.RemainSize < syncTaskModel.FileEntity.Size)
                        {
                            _ = storgaeService.AddFileEntityInStorageAsync(new FileEntityStroage()
                            {
                                CreatorIdentity = _option.Identity,
                                FileEntityId = syncTaskModel.FileEntity.FileEntityId,
                                Status = FileEntityStorageStatusEnum.SyncFail,
                                StorageId = desStorage.Storage.StorageId,
                                Value = "Sync Fail - Remain space not enough"
                            }).Result;
                            continue;
                        }

                        var fromProvider = fileSystemFacotry.GetAsyncFileProvider(syncTaskModel.FromFileEntityStorage.Storage);
                        var desProvider = fileSystemFacotry.GetAsyncFileProvider(desStorage.Storage);
                        var fileInfo = fromProvider.GetFileInfo(syncTaskModel.FromFileEntityStorage.Value);
                        Guid taskId = Guid.NewGuid();

                        var desFileEntityStorage = storgaeService.AddFileEntityInStorageAsync(new FileEntityStroage()
                        {
                            CreatorIdentity = _option.Identity,
                            FileEntityId = syncTaskModel.FileEntity.FileEntityId,
                            Status = FileEntityStorageStatusEnum.Syncing,
                            StorageId = desStorage.Storage.StorageId,
                            Value = taskId.ToString()
                        }).Result;

                        _logger.LogInformation(_option.Identity, "同步開始", sysncTaskIdentity
                            , "正在將檔案 ID 為 {0} 的 {1} 從 {2} 群組中的 {3} 儲存個體 同步至 ---> {4} 儲存群組中的 {5} 儲存個體",
                            syncTaskModel.FileEntity.FileEntityId,
                            syncTaskModel.FileEntity.Name,
                            syncTaskModel.FromStorageGroup.Name,
                            syncTaskModel.FromFileEntityStorage.Storage.Name,
                            syncTaskModel.DestinationStorageGroup.Name,
                            desStorage.Storage.Name);

                        var desFileIno = desProvider.PutAsync(desFileEntityStorage.Value, fileInfo.CreateReadStream()).Result;

                        storgaeService.CompleteSyncAsync(desFileEntityStorage.FileEntityStroageId, desFileIno.Name).Wait();

                        _logger.LogInformation(_option.Identity, "同步完成", sysncTaskIdentity
                            , "檔案 ID 為 {0} 的 {1} 從 {2} 群組中的 {3} 儲存個體 同步至 ---> {4} 儲存群組中的 {5} 儲存個體",
                            syncTaskModel.FileEntity.FileEntityId,
                            syncTaskModel.FileEntity.Name,
                            syncTaskModel.FromStorageGroup.Name,
                            syncTaskModel.FromFileEntityStorage.Storage.Name,
                            syncTaskModel.DestinationStorageGroup.Name,
                            desStorage.Storage.Name);
                    }
                }
                catch (Exception ex)
                {
                    LoggerExtensions.LogError(_logger, _option.Identity, "同步失敗", sysncTaskIdentity, ex, "發生未預期的例外");
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
