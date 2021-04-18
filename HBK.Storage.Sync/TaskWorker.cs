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
    /// ���Ȱ����
    /// </summary>
    public class TaskWorker : BackgroundService
    {
        private readonly ILogger<TaskWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _scope;
        /// <summary>
        /// �إߤ@�ӷs���������
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
            this.ExpireFileEntityTaskManager = _scope.ServiceProvider.GetRequiredService<ExpireFileEntityTaskManager>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("�A�Ȱ���(ExecuteAsync)");

            try
            {
                this.SyncTaskManager.Start(stoppingToken);
                this.DeleteFileEntityTaskManager.Start(stoppingToken);
                this.ExpireFileEntityTaskManager.Start(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "�A�ȵo�ͥ��w�����ҥ~");
            }
            return Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("�A�ȱҰ�(StartAsync)");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("�A�ȼȰ�(StopAsync)");
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("�A������(Dispose)");
            base.Dispose();
        }

        public SyncTaskManager SyncTaskManager { get; private set; }
        public DeleteFileEntityTaskManager DeleteFileEntityTaskManager { get; private set; }
        public ExpireFileEntityTaskManager ExpireFileEntityTaskManager { get; private set; }
    }
}
