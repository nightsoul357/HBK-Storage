using Amazon.Runtime.Internal.Util;
using HBK.Storage.Adapter.Enums;
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
    /// 同步任務管理者
    /// </summary>
    public sealed class SyncTaskManager : TaskManagerBase<SyncTaskManager, SyncTaskManagerOption>
    {
        private ConcurrentQueue<SyncTaskModel> _pendingQueue;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public SyncTaskManager(ILogger<SyncTaskManager> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }
        protected override void StartInternal()
        {
            _pendingQueue = new ConcurrentQueue<SyncTaskModel>();

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
                    var tasks = storageProviderService.GetSyncFileEntitiesAsync(storageProvider.StorageProviderId,
                        base.Option.FetchLimit - _pendingQueue.Count,
                        base.Option.FileEntityNoDivisor,
                        base.Option.FileEntityNoRemainder).Result;

                    tasks.ForEach(x => _pendingQueue.Enqueue(new SyncTaskModel()
                    {
                        CreateDateTime = DateTimeOffset.Now,
                        DestinationStorageGroup = x.DestinationStorageGroup,
                        FileEntity = x.FileEntity,
                        FromFileEntityStorage = x.FromFileEntityStorage,
                        FromStorageGroup = x.FromStorageGroup
                    }));

                    if (_pendingQueue.Count >= base.Option.FetchLimit)
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
                using (var scope = _serviceProvider.CreateScope())
                {
                    var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                    var storageGroupService = scope.ServiceProvider.GetRequiredService<StorageGroupService>();
                    var storgaeService = scope.ServiceProvider.GetRequiredService<StorageService>();
                    var fileSystemFacotry = scope.ServiceProvider.GetRequiredService<FileSystemFactory>();
                    var fileEntityStorageService = scope.ServiceProvider.GetRequiredService<FileEntityStorageService>();

                    try
                    {
                        var desStorage = storageGroupService.GetMaxRemainSizeStorageByStorageGroupIdAsync(syncTaskModel.DestinationStorageGroup.StorageGroupId).Result;
                        if (desStorage == null)
                        {
                            storageGroupService.DisableStorageGroupAsync(syncTaskModel.DestinationStorageGroup.StorageGroupId).Wait();

                            base.LogWarn(sysncTaskIdentity, syncTaskModel.FileEntity, null, "不存在有效的儲存個體 - {0} 儲存群組中不存在有效的儲存個體，故該儲存群組已被關閉",
                                syncTaskModel.DestinationStorageGroup.Name);
                            continue;
                        }
                        if (desStorage.RemainSize < syncTaskModel.FileEntity.Size)
                        {
                            _ = storgaeService.AddFileEntityInStorageAsync(new FileEntityStorage()
                            {
                                CreatorIdentity = base.Option.Identity,
                                FileEntityId = syncTaskModel.FileEntity.FileEntityId,
                                Status = FileEntityStorageStatusEnum.SyncFail,
                                StorageId = desStorage.Storage.StorageId,
                                Value = "Sync Fail - Remain space not enough"
                            }).Result;

                            base.LogWarn(sysncTaskIdentity, syncTaskModel.FileEntity, null, "嘗試從 {0} 群組中的 {1} 儲存個體 同步至 ---> {2} 儲存群組中的 {3} 儲存個體 但空間不足",
                                syncTaskModel.FromStorageGroup.Name,
                                syncTaskModel.FromFileEntityStorage.Storage.Name,
                                syncTaskModel.DestinationStorageGroup.Name,
                                desStorage.Storage.Name);

                            continue;
                        }

                        IAsyncFileInfo fileInfo;
                        if ((fileInfo = fileEntityStorageService.TryFetchFileInfoAsync(syncTaskModel.FromFileEntityStorage.FileEntityStorageId).Result) == null)
                        {
                            base.LogWarn(sysncTaskIdentity, syncTaskModel.FileEntity, null, "嘗試從 {0} 群組中的 {1} 儲存個體 同步至 ---> {2} 儲存群組中的 {3} 儲存個體 但該檔案無法正確存取",
                                syncTaskModel.FromStorageGroup.Name,
                                syncTaskModel.FromFileEntityStorage.Storage.Name,
                                syncTaskModel.DestinationStorageGroup.Name,
                                desStorage.Storage.Name);

                            continue; // 檔案無效，執行下一筆
                        }

                        var desProvider = fileSystemFacotry.GetAsyncFileProvider(desStorage.Storage);
                        Guid taskId = Guid.NewGuid();

                        var desFileEntityStorage = storgaeService.AddFileEntityInStorageAsync(new FileEntityStorage()
                        {
                            CreatorIdentity = base.Option.Identity,
                            FileEntityId = syncTaskModel.FileEntity.FileEntityId,
                            Status = FileEntityStorageStatusEnum.Syncing,
                            StorageId = desStorage.Storage.StorageId,
                            Value = taskId.ToString()
                        }).Result;

                        syncTaskModel.DestinationFileEntityStorage = desFileEntityStorage;

                        base.LogInformation(sysncTaskIdentity, syncTaskModel.FileEntity, null, "[同步開始]正在從 {0} 群組中的 {1} 儲存個體 同步至 ---> {2} 儲存群組中的 {3} 儲存個體",
                            syncTaskModel.FromStorageGroup.Name,
                            syncTaskModel.FromFileEntityStorage.Storage.Name,
                            syncTaskModel.DestinationStorageGroup.Name,
                            desStorage.Storage.Name);

                        var desFileIno = desProvider.PutAsync(desFileEntityStorage.Value, fileInfo.CreateReadStream()).Result;

                        storgaeService.CompleteSyncAsync(desFileEntityStorage.FileEntityStorageId, desFileIno.Name).Wait();

                        base.LogInformation(sysncTaskIdentity, syncTaskModel.FileEntity, null, "[同步完成]正在從 {0} 群組中的 {1} 儲存個體 同步至 ---> {2} 儲存群組中的 {3} 儲存個體",
                            syncTaskModel.FromStorageGroup.Name,
                            syncTaskModel.FromFileEntityStorage.Storage.Name,
                            syncTaskModel.DestinationStorageGroup.Name,
                            desStorage.Storage.Name);

                        fileEntityStorageService.AddSyncSuccessfullyRecordAsync(syncTaskModel.FromFileEntityStorage.FileEntityStorageId, String.Empty, syncTaskModel.DestinationFileEntityStorage.StorageId).Wait();
                    }
                    catch (Exception ex)
                    {
                        base.LogError(sysncTaskIdentity, syncTaskModel.FileEntity, null, ex, "發生未預期的例外");
                        fileEntityStorageService.AddSyncFailRecordAsync(syncTaskModel.FromFileEntityStorage.FileEntityStorageId, ex.Message, syncTaskModel.DestinationFileEntityStorage.StorageId).Wait();
                        storgaeService.RevorkSyncAsync(syncTaskModel.DestinationFileEntityStorage.FileEntityStorageId).Wait();
                    }
                }
            }
        }
        public override void Dispose()
        {

        }
    }
}
