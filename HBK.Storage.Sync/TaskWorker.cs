using HBK.Storage.Sync.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.Sync
{
    /// <summary>
    /// 任務執行者
    /// </summary>
    public class TaskWorker : BackgroundService
    {
        private readonly ILogger<TaskWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _scope;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public TaskWorker(ILogger<TaskWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _scope = _serviceProvider.CreateScope();
            this.SyncTaskManager = _scope.ServiceProvider.GetRequiredService<SyncTaskManager>();
            this.DeleteFileEntityTaskManager = _scope.ServiceProvider.GetRequiredService<DeleteFileEntityTaskManager>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                this.SyncTaskManager.Start(stoppingToken);
                this.DeleteFileEntityTaskManager.Start(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uncath Error");
            }
            return Task.CompletedTask;
        }

        public SyncTaskManager SyncTaskManager { get; private set; }
        public DeleteFileEntityTaskManager DeleteFileEntityTaskManager { get; private set; }
    }
}
