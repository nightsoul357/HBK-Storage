using HBK.Storage.ImageCompressPlugin;
using HBK.Storage.VideoConvertM3U8Plugin;
using HBK.Storage.VideoMetadataPlugin;
using HBK.Storage.VideoSubTitleCombinePlugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.PluginIntegration
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
            this.ImageCompressTaskManager = _scope.ServiceProvider.GetRequiredService<ImageCompressTaskManager>();
            this.VideoConvertM3U8TaskManager = _scope.ServiceProvider.GetRequiredService<VideoConvertM3U8TaskManager>();
            this.VideoMetadataTaskManager = _scope.ServiceProvider.GetRequiredService<VideoMetadataTaskManager>();
            this.VideoSubTitleCombineTaskManager = _scope.ServiceProvider.GetRequiredService<VideoSubTitleCombineTaskManager>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("�A�Ȱ���(ExecuteAsync)");

            try
            {
                this.ImageCompressTaskManager.Start(stoppingToken);
                this.VideoConvertM3U8TaskManager.Start(stoppingToken);
                this.VideoMetadataTaskManager.Start(stoppingToken);
                this.VideoSubTitleCombineTaskManager.Start(stoppingToken);
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

        public ImageCompressTaskManager ImageCompressTaskManager { get; set; }
        public VideoConvertM3U8TaskManager VideoConvertM3U8TaskManager { get; set; }
        public VideoMetadataTaskManager VideoMetadataTaskManager { get; set; }
        public VideoSubTitleCombineTaskManager VideoSubTitleCombineTaskManager { get; set; }
    }
}