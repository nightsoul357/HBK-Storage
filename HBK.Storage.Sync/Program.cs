using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.Sync.Managers;
using HBK.Storage.Sync.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Sync
{
    /// <summary>
    /// 程式主類別
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 應用程式進入點
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 建立主機
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var scope = services.BuildServiceProvider().CreateScope();
                    IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    services.AddDbContext<HBKStorageContext>(options =>
                        options.UseSqlServer(configuration["Database:ConnectionString"]));

                    services.AddHBKStorageService();

                    var configForSyncTaskManagerOption = configuration.GetSection("SyncTaskManagerOption");
                    services.AddSingleton(sp =>
                    {
                        var option = new SyncTaskManagerOption();
                        configForSyncTaskManagerOption.Bind(option);
                        return option;
                    });

                    var configForDeleteFileEntityTaskManagerOption = configuration.GetSection("DeleteFileEntityTaskManagerOption");
                    services.AddSingleton(sp =>
                    {
                        var option = new DeleteFileEntityTaskManagerOption();
                        configForDeleteFileEntityTaskManagerOption.Bind(option);
                        return option;
                    });

                    services.AddSingleton<SyncTaskManager>();
                    services.AddSingleton<DeleteFileEntityTaskManager>();

                    services.AddHostedService<TaskWorker>();
                });
    }
}
