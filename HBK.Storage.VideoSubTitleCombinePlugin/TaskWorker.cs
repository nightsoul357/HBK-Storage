using HBK.Storage.VideoSubTitleCombinePlugin.CombineHandler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSubTitleCombinePlugin
{
    public class TaskWorker : BackgroundService
    {
        private readonly ILogger<TaskWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _scope;

        public TaskWorker(ILogger<TaskWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _scope = _serviceProvider.CreateScope();
            this.VideoSubTitleCombineTaskManager = _scope.ServiceProvider.GetRequiredService<VideoSubTitleCombineTaskManager>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("服務執行(ExecuteAsync)");

            try
            {
                this.VideoSubTitleCombineTaskManager.Start(stoppingToken);
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
        public VideoSubTitleCombineTaskManager VideoSubTitleCombineTaskManager { get; set; }
    }
}
