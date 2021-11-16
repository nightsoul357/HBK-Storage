using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core;
using HBK.Storage.Core.Cryptography;
using HBK.Storage.ImageCompressPlugin.Models;
using HBK.Storage.PluginCore.NLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.LayoutRenderers;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.ImageCompressPlugin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Program.CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var scope = services.BuildServiceProvider().CreateScope();
                    IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                    services.AddDbContext<HBKStorageContext>(options =>
                        options.UseSqlServer(configuration["Database:ConnectionString"]));

                    var configurationForImageCompressTaskOptions = configuration.GetSection("ImageCompressTaskOptions");
                    services.AddSingleton(sp =>
                    {
                        var option = new ImageCompressTaskOptions();
                        configurationForImageCompressTaskOptions.Bind(option);
                        return option;
                    });

                    LayoutRenderer.Register<PluginIdentityLayoutRenderer>("plugin_identity");
                    LayoutRenderer.Register<PluginFileEntityFilenameLayoutRenderer>("plugin_file_entity_filename");
                    LayoutRenderer.Register<PluginFileEntityIdLayoutRenderer>("plugin_file_entity_id");
                    LayoutRenderer.Register<PluginActivityIdReanderer>("plugin_activityId");

                    services.AddHBKStorageService();

                    services.AddScoped<ICryptoProvider, AESCryptoProvider>();

                    services.AddSingleton<ImageCompressTaskManager>();
                    services.AddHostedService<TaskWorker>();
                })
                .UseNLog();
    }
}
