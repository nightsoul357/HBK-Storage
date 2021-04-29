using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core;
using HBK.Storage.VideoSubTitleCombinePlugin.CombineHandler;
using HBK.Storage.VideoSubTitleCombinePlugin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSubTitleCombinePlugin
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

                    var configurationForVideoSubTitleCombineTaskOptions = configuration.GetSection("VideoSubTitleCombineTaskOptions");
                    services.AddSingleton(sp =>
                    {
                        var option = new VideoSubTitleCombineTaskOptions();
                        configurationForVideoSubTitleCombineTaskOptions.Bind(option);
                        return option;
                    });

                    services.AddHBKStorageService();

                    services.AddScoped<CombineHandlerBase>((sp =>
                    {
                        return new ClearCombineHandler(sp.GetRequiredService<ILogger<ClearCombineHandler>>(),
                            new QueueSizeCombineHandler(sp.GetRequiredService<ILogger<QueueSizeCombineHandler>>()));
                    }));

                    services.AddSingleton<VideoSubTitleCombineTaskManager>();
                    services.AddHostedService<TaskWorker>();
                });
    }
}
