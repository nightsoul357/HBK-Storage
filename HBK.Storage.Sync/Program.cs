using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using HBK.Storage.Sync.Managers;
using HBK.Storage.Sync.Model;
using HBK.Storage.Sync.NLog;
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
            Program.CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 建立主機
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
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

                    var configForExpireFileEntityTaskManagerOption = configuration.GetSection("ExpireFileEntityTaskManagerOption");
                    services.AddSingleton(sp =>
                    {
                        var option = new ExpireFileEntityTaskManagerOption();
                        configForExpireFileEntityTaskManagerOption.Bind(option);
                        return option;
                    });

                    var configForClearTaskManagerOption = configuration.GetSection("ClearTaskManagerOption");
                    services.AddSingleton(sp =>
                    {
                        var option = new ClearTaskManagerOption();
                        configForClearTaskManagerOption.Bind(option);
                        return option;
                    });

                    LayoutRenderer.Register<PluginIdentityLayoutRenderer>("plugin_identity");
                    LayoutRenderer.Register<PluginFileEntityFilenameLayoutRenderer>("plugin_file_entity_filename");
                    LayoutRenderer.Register<PluginFileEntityIdLayoutRenderer>("plugin_file_entity_id");
                    LayoutRenderer.Register<PluginActivityIdReanderer>("plugin_activityId");

                    services.AddSingleton<SyncTaskManager>();
                    services.AddSingleton<DeleteFileEntityTaskManager>();
                    services.AddSingleton<ExpireFileEntityTaskManager>();
                    services.AddSingleton<ClearTaskManager>();

                    services.AddHostedService<TaskWorker>();
                })
                .UseWindowsService()
                .UseNLog();
    }
}