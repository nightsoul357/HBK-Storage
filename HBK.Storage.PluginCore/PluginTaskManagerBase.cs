using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HBK.Storage.PluginCore
{
    public abstract class PluginTaskManagerBase<T, TOption> : IDisposable
        where TOption : PluginTaskManagerOptions
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger<T> _logger;
        protected CancellationToken _cancellationToken;
        protected readonly IServiceScope _serviceScope;

        private ConcurrentQueue<PluginTaskModel> _pendingQueue;
        private readonly object _syncObj;
        private bool _isDisposed = false;
        public PluginTaskManagerBase(ILogger<T> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceScope = serviceProvider.CreateScope();
            _pendingQueue = new ConcurrentQueue<PluginTaskModel>();
            _syncObj = new object();
            this.Options = _serviceScope.ServiceProvider.GetRequiredService<TOption>();
            this.IsRunning = false;
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
                        Task.Factory.StartNewSafety(this.StartInternal,
                            TaskCreationOptions.LongRunning,
                            (Exception ex) =>
                            {
                                this.ExcetpionHandle(ex);
                                this.IsRunning = false;
                            });

                        this.IsRunning = true;
                    }
                }
            }
        }
        private void StartInternal()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                this.Fetch();
                if (!_pendingQueue.IsEmpty)
                {
                    List<Task> tasks = new List<Task>();
                    for (int i = 0; i < this.Options.TaskLimit; i++)
                    {
                        tasks.Add(Task.Factory.StartNewSafety(this.ExecuteTask, TaskCreationOptions.LongRunning, this.ExcetpionHandle));
                    }

                    Task.WaitAll(tasks.ToArray());
                }
                else
                {
                    SpinWait.SpinUntil(() => false, this.Options.Interval);
                }
            }
        }
        private void Fetch()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var storageProviderService = scope.ServiceProvider.GetRequiredService<StorageProviderService>();
                List<StorageProvider> storageProviders = new List<StorageProvider>();
                if (this.Options.IsExecutOnAllStoragProvider)
                {
                    storageProviders.AddRange(storageProviderService.GetAllStorageProviderAsync().Result);
                }
                else
                {
                    storageProviders.AddRange(storageProviderService.GetStorageProviderByIdsAsync(this.Options.StorageProviderIds).Result);
                }

                foreach (var storageProvider in storageProviders)
                {
                    List<FileEntity> fileEntites = null;
                    if (this.Options.FetchMode == FetchModeEnum.WithoutTag)
                    {
                        fileEntites = storageProviderService.GetFileEntityWithoutTagAsync(
                        storageProviderId: storageProvider.StorageProviderId,
                        tag: this.Options.IdentityTag,
                        mimeTypeParten: this.Options.MimeTypePartten,
                        isRootFileEntity: this.Options.JustExecuteOnRootFileEntity,
                        takeCount: this.Options.FetchLimit - _pendingQueue.Count,
                        fileEntityNoDivisor: this.Options.FileEntityNoDivisor,
                        fileEntityNoRemainder: this.Options.FileEntityNoRemainder).Result;
                    }
                    else if (this.Options.FetchMode == FetchModeEnum.WithTag)
                    {
                        fileEntites = storageProviderService.GetFileEntityWithTagAsync(
                        storageProviderId: storageProvider.StorageProviderId,
                        tag: this.Options.IdentityTag,
                        mimeTypeParten: this.Options.MimeTypePartten,
                        isRootFileEntity: this.Options.JustExecuteOnRootFileEntity,
                        takeCount: this.Options.FetchLimit - _pendingQueue.Count,
                        fileEntityNoDivisor: this.Options.FileEntityNoDivisor,
                        fileEntityNoRemainder: this.Options.FileEntityNoRemainder).Result;
                    }

                    fileEntites.ForEach(x => _pendingQueue.Enqueue(new PluginTaskModel()
                    {
                        FileEntity = x,
                        StorageProviderId = storageProvider.StorageProviderId
                    }));

                    if (_pendingQueue.Count >= this.Options.FetchLimit)
                    {
                        break;
                    }
                }
            }
        }
        private void ExecuteTask()
        {
            while (!_cancellationToken.IsCancellationRequested && _pendingQueue.TryDequeue(out PluginTaskModel task))
            {
                try
                {
                    if (!this.ExecuteInternal(task))
                    {
                        this.ErrorFileEntity(task.FileEntity.FileEntityId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[{0}] 檔案 ID -> {1} 執行時發生未預期的例外", this.Options.Identity, task.FileEntity.FileEntityId);
                    this.ErrorFileEntity(task.FileEntity.FileEntityId);
                }
                finally
                {
                    this.CompleteFileEntity(task.FileEntity.FileEntityId);
                }
            }
        }
        protected void CompleteFileEntity(Guid fileEntityId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();

                if (this.Options.FetchMode == FetchModeEnum.WithoutTag)
                {
                    fileEntityService.AppendTagAsync(fileEntityId, this.Options.IdentityTag).Wait();
                }
                else if (this.Options.FetchMode == FetchModeEnum.WithTag)
                {
                    fileEntityService.RemoveTagAsync(fileEntityId, this.Options.IdentityTag).Wait();
                }
            }
        }
        protected void ErrorFileEntity(Guid fileEntityId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var fileEntityService = scope.ServiceProvider.GetRequiredService<FileEntityService>();

                if (this.Options.FetchMode == FetchModeEnum.WithoutTag)
                {
                    fileEntityService.AppendTagAsync(fileEntityId, this.Options.ExceptionTag).Wait();
                }
                else if (this.Options.FetchMode == FetchModeEnum.WithTag)
                {
                    fileEntityService.AppendTagAsync(fileEntityId, this.Options.ExceptionTag).Wait();
                }
            }
        }
        /// <summary>
        /// 移除殘留檔案
        /// </summary>
        /// <param name="fileEntityService"></param>
        /// <param name="taskModel"></param>
        protected void RemoveResidueFileEntity(FileEntityService fileEntityService, PluginTaskModel taskModel)
        {
            var remove = fileEntityService.GetChildFileEntitiesAsync(taskModel.FileEntity.FileEntityId).Result
                    .Where(x => x.Status.HasFlag(FileEntityStatusEnum.Processing) && x.FileEntityTag.Any(t => t.Value == this.Options.Identity));

            if (remove.Count() != 0)
            {
                fileEntityService.MarkFileEntityDeleteBatchAsync(remove.Select(x => x.FileEntityId).ToList()).Wait();
                _logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 在處理的過程中發現了 {3} 個殘留的檔案，以標記為移除", this.Options.Identity, taskModel.FileEntity.FileEntityId, taskModel.FileEntity.Name, remove.Count());
            }
        }

        protected void DownloadFileEntity(IAsyncFileInfo downloadfile, FileEntity fileEntity, string savePath)
        {
            var sourceStream = downloadfile.CreateReadStream();
            _logger.LogInformation("[{0}] 開始下載檔案 ID {1} 的 {2} 檔案", this.Options.Identity, fileEntity.FileEntityId, fileEntity.Name);
            using (var fstream = File.Create(savePath))
            {
                sourceStream.CopyTo(fstream);
            }
            _logger.LogInformation("[{0}] 檔案 ID {1} 的 {2} 檔案 下載完成", this.Options.Identity, fileEntity.FileEntityId, fileEntity.Name);
        }
        protected abstract bool ExecuteInternal(PluginTaskModel taskModel);
        /// <summary>
        /// 未攔截的例外處理方式
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void ExcetpionHandle(Exception ex)
        {
            _logger.LogError(ex, "發生未預期的例外");
        }

        public virtual void Dispose()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("物件已被釋放");
            }

            _serviceScope.Dispose();
            _isDisposed = true;
            this.IsRunning = false;
        }

        public TOption Options { get; private set; }
        public bool IsRunning { get; protected set; }
    }
}
