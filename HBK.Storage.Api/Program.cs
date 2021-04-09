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
    /// �{���D���O
    /// </summary>
    public class Program
    {
        /// <summary>
        /// ���ε{���i�J�I
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Program.CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// �إߥD��
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
