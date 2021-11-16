using HBK.Storage.Adapter.Storages;
using HBK.Storage.Sync.Extensions;
using HBK.Storage.Sync.Model;
using HBK.Storage.Sync.NLog;
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
    /// 任務管理者基底型別
    /// </summary>
    public abstract class TaskManagerBase<T, TOption> : IDisposable
        where TOption : TaskMangagerOptionBase
    {
        protected readonly ILogger<T> _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected CancellationToken _cancellationToken;
        private object _syncObj = new object();
        private readonly IServiceScope _scope;
        private Guid _executeId;

        public TaskManagerBase(ILogger<T> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _scope = _serviceProvider.CreateScope();
            this.Option = _scope.ServiceProvider.GetRequiredService<TOption>();
            this.IsRunning = false;
        }
        /// <summary>
        /// 啟動任務管理器
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Start(CancellationToken cancellationToken)
        {
            if (!this.IsRunning)
            {
                lock (_syncObj)
                {
                    if (!this.IsRunning)
                    {
                        _executeId = Guid.NewGuid();
                        this.LogInformation(_executeId, null, null, "服務啟動");
                        _cancellationToken = cancellationToken;
                        Task.Factory.StartNewSafety(this.StartInternal,
                            TaskCreationOptions.LongRunning,
                            (ex) =>
                            {
                                this.ExcetpionHandle(ex);
                                this.IsRunning = false;
                                if (this.Option.IsAutoRetry)
                                {
                                    this.LogInformation(_executeId, null, null, $"{this.Option.AutoRetryInterval / 1000} 秒後嘗試重新啟動...");
                                    SpinWait.SpinUntil(() => false, this.Option.AutoRetryInterval);
                                    this.Start(cancellationToken);
                                }
                                else
                                {
                                    this.LogInformation(_executeId, null, null, "服務終止");
                                }
                            });
                        this.IsRunning = true;
                    }
                }
            }
        }
        /// <summary>
        /// 內部啟動方式
        /// </summary>
        protected abstract void StartInternal();
        /// <summary>
        /// 未攔截的例外處理方式
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void ExcetpionHandle(Exception ex)
        {
            this.LogError(_executeId, null, null, ex, "發生未預期的例外");
        }
        /// <summary>
        /// 紀錄 Info 等級的訊息
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="fileEntity"></param>
        /// <param name="extendProperty"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        protected void LogInformation(Guid taskId, FileEntity fileEntity, object extendProperty, string message, params object[] args)
        {
            _logger.LogInformation(LogEvent.BuildPluginLogEvent(this.Option.Identity, taskId, fileEntity, extendProperty), message, args);
        }

        /// <summary>
        /// 紀錄 Warn 等級的訊息
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="fileEntity"></param>
        /// <param name="extendProperty"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        protected void LogWarn(Guid taskId, FileEntity fileEntity, object extendProperty, string message, params object[] args)
        {
            _logger.LogWarning(LogEvent.BuildPluginLogEvent(this.Option.Identity, taskId, fileEntity, extendProperty), message, args);
        }

        /// <summary>
        /// 紀錄 Error 等級的訊息
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="fileEntity"></param>
        /// <param name="extendProperty"></param>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        protected void LogError(Guid taskId, FileEntity fileEntity, object extendProperty, Exception ex, string message, params object[] args)
        {
            _logger.LogError(LogEvent.BuildPluginLogEvent(this.Option.Identity, taskId, fileEntity, extendProperty), ex, message, args);
        }

        public abstract void Dispose();
        /// <summary>
        /// 取得任務是否正在執行中
        /// </summary>
        public bool IsRunning { get; protected set; }
        /// <summary>
        /// 取得設定檔
        /// </summary>
        public TOption Option { get; private set; }
    }
}