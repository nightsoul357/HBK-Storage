using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core;
using HBK.Storage.VideoSeekPreviewPlugin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSeekPreviewPlugin
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

                    var configurationForVideoSeekPreviewTaskManagerOptions = configuration.GetSection("VideoSeekPreviewTaskManagerOptions");
                    services.AddSingleton(sp =>
                    {
                        var option = new VideoSeekPreviewTaskManagerOptions();
                        configurationForVideoSeekPreviewTaskManagerOptions.Bind(option);
                        return option;
                    });

                    services.AddHBKStorageService();

                    services.AddSingleton<VideoSeekPreviewTaskManager>();

                    services.AddHostedService<TaskWorker>();
                });
    }
}
