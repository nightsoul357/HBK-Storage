using HBK.Storage.Sync.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IHostEnvironment _hostEnvironment;

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
            _hostEnvironment = _scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
            this.SyncTaskManager = _scope.ServiceProvider.GetRequiredService<SyncTaskManager>();
            this.DeleteFileEntityTaskManager = _scope.ServiceProvider.GetRequiredService<DeleteFileEntityTaskManager>();
            this.ExpireFileEntityTaskManager = _scope.ServiceProvider.GetRequiredService<ExpireFileEntityTaskManager>();

            Directory.SetCurrentDirectory(_hostEnvironment.ContentRootPath);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("服務執行(ExecuteAsync)");

            try
            {
                this.SyncTaskManager.Start(stoppingToken);
                this.DeleteFileEntityTaskManager.Start(stoppingToken);
                this.ExpireFileEntityTaskManager.Start(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "服務發生未預期的例外");
            }
            return Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("服務啟動(StartAsync)");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("服務暫停(StopAsync)");
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("服務釋放(Dispose)");
            base.Dispose();
        }

        public SyncTaskManager SyncTaskManager { get; private set; }
        public DeleteFileEntityTaskManager DeleteFileEntityTaskManager { get; private set; }
        public ExpireFileEntityTaskManager ExpireFileEntityTaskManager { get; private set; }
    }
}
