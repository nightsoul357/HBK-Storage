using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
