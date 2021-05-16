using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core;
using HBK.Storage.ImageCompressPlugin;
using HBK.Storage.ImageCompressPlugin.Models;
using HBK.Storage.PluginCore.NLog;
using HBK.Storage.VideoConvertM3U8Plugin;
using HBK.Storage.VideoConvertM3U8Plugin.ConvertHandler;
using HBK.Storage.VideoConvertM3U8Plugin.Models;
using HBK.Storage.VideoMetadataPlugin;
using HBK.Storage.VideoMetadataPlugin.Models;
using HBK.Storage.VideoSeekPreviewPlugin;
using HBK.Storage.VideoSeekPreviewPlugin.Models;
using HBK.Storage.VideoSubTitleCombinePlugin;
using HBK.Storage.VideoSubTitleCombinePlugin.CombineHandler;
using HBK.Storage.VideoSubTitleCombinePlugin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.LayoutRenderers;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.PluginIntegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var scope = services.BuildServiceProvider().CreateScope();
                    IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    // 資料庫
                    services.AddDbContext<HBKStorageContext>(options =>
                        options.UseSqlServer(configuration["Database:ConnectionString"]));

                    // 設定檔
                    var configurationForImageCompressTaskOptions = configuration.GetSection("ImageCompressTaskOptions");
                    services.AddSingleton(sp =>
                    {
                        var option = new ImageCompressTaskOptions();
                        configurationForImageCompressTaskOptions.Bind(option);
                        return option;
                    });

                    var configurationForVideoConvertM3U8TaskManagerOptions = configuration.GetSection("VideoConvertM3U8TaskManagerOptions");
                    services.AddSingleton(sp =>
                    {
                        var option = new VideoConvertM3U8TaskManagerOptions();
                        configurationForVideoConvertM3U8TaskManagerOptions.Bind(option);
                        return option;
                    });

                    var configurationForVideoMetadataTaskManagerOptions = configuration.GetSection("VideoMetadataTaskManagerOptions");
                    services.AddSingleton(sp =>
                    {
                        var option = new VideoMetadataTaskManagerOptions();
                        configurationForVideoMetadataTaskManagerOptions.Bind(option);
                        return option;
                    });

                    var configurationForVideoSubTitleCombineTaskOptions = configuration.GetSection("VideoSubTitleCombineTaskOptions");
                    services.AddSingleton(sp =>
                    {
                        var option = new VideoSubTitleCombineTaskOptions();
                        configurationForVideoSubTitleCombineTaskOptions.Bind(option);
                        return option;
                    });

                    var configurationForVideoSeekPreviewTaskManagerOptions = configuration.GetSection("VideoSeekPreviewTaskManagerOptions");
                    services.AddSingleton(sp =>
                    {
                        var option = new VideoSeekPreviewTaskManagerOptions();
                        configurationForVideoSeekPreviewTaskManagerOptions.Bind(option);
                        return option;
                    });

                    // 核心服務
                    services.AddHBKStorageService();

                    LayoutRenderer.Register<PluginIdentityLayoutRenderer>("plugin_identity");
                    LayoutRenderer.Register<PluginFileEntityFilenameLayoutRenderer>("plugin_file_entity_filename");
                    LayoutRenderer.Register<PluginFileEntityIdLayoutRenderer>("plugin_file_entity_id");

                    // 轉換器
                    services.AddScoped<ConvertHandlerBase>((sp =>
                    {
                        return new CuvidConvertHandler(sp.GetRequiredService<ILogger<ConvertHandlerBase>>(),
                            new QueueSizeConvertHandler(sp.GetRequiredService<ILogger<QueueSizeConvertHandler>>(),
                                new ClearConvertHandler(sp.GetRequiredService<ILogger<ClearConvertHandler>>(),
                                    new EmptyConvertHandler(sp.GetRequiredService<ILogger<EmptyConvertHandler>>()))));
                    }));
                    services.AddScoped<CombineHandlerBase>((sp =>
                    {
                        return new ClearCombineHandler(sp.GetRequiredService<ILogger<ClearCombineHandler>>(),
                            new QueueSizeCombineHandler(sp.GetRequiredService<ILogger<QueueSizeCombineHandler>>()));
                    }));


                    // 任務管理器
                    services.AddSingleton<ImageCompressTaskManager>();
                    services.AddSingleton<VideoConvertM3U8TaskManager>();
                    services.AddSingleton<VideoMetadataTaskManager>();
                    services.AddSingleton<VideoSubTitleCombineTaskManager>();
                    services.AddSingleton<VideoSeekPreviewTaskManager>();

                    services.AddHostedService<TaskWorker>();
                })
                .UseWindowsService()
                .UseNLog();
    }
}
