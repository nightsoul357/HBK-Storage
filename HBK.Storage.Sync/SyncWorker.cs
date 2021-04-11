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
    /// �P�B�A��
    /// </summary>
    public class SyncWorker : BackgroundService
    {
        private readonly ILogger<SyncWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _scope;
        /// <summary>
        /// �إߤ@�ӷs���������
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public SyncWorker(ILogger<SyncWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _scope = _serviceProvider.CreateScope();
            this.SyncTaskManager = _scope.ServiceProvider.GetRequiredService<SyncTaskManager>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.SyncTaskManager.Start(stoppingToken);
            return Task.CompletedTask;
        }

        public SyncTaskManager SyncTaskManager { get; private set; }
    }
}
