using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core;
using HBK.Storage.VideoConvertM3U8Plugin.ConvertHandler;
using HBK.Storage.VideoConvertM3U8Plugin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.VideoConvertM3U8Plugin
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

                    services.AddDbContext<HBKStorageContext>(options =>
                        options.UseSqlServer(configuration["Database:ConnectionString"]));

                    var configurationForVideoConvertM3U8TaskManagerOptions = configuration.GetSection("VideoConvertM3U8TaskManagerOptions");
                    services.AddSingleton(sp =>
                    {
                        var option = new VideoConvertM3U8TaskManagerOptions();
                        configurationForVideoConvertM3U8TaskManagerOptions.Bind(option);
                        return option;
                    });

                    services.AddHBKStorageService();

                    services.AddScoped<ConvertHandlerBase>((sp =>
                    {
                        return new CuvidConvertHandler(sp.GetRequiredService<ILogger<ConvertHandlerBase>>());
                    }));

                    services.AddSingleton<VideoConvertM3U8TaskManager>();

                    services.AddHostedService<TaskWorker>();
                });
    }
}
