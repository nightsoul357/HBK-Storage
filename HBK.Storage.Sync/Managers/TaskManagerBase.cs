using HBK.Storage.Sync.Extensions;
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
    public abstract class TaskManagerBase<T> : IDisposable
    {
        protected readonly ILogger<T> _logger;
        protected readonly IServiceProvider _serviceProvider;

        protected CancellationToken _cancellationToken;
        private object _syncObj = new object();

        public TaskManagerBase(ILogger<T> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
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
                        _cancellationToken = cancellationToken;
                        _cancellationToken.Register(this.Dispose);
                        Task.Factory.StartNewSafety(this.StartInternal,
                            TaskCreationOptions.LongRunning,
                            this.ExcetpionHandle);
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
            _logger.LogError(ex, "發生未預期的例外");
        }

        public abstract void Dispose();
        /// <summary>
        /// 取得任務是否正在執行中
        /// </summary>
        public bool IsRunning { get; protected set; }
    }
}
