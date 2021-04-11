using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core
{
    /// <summary>
    /// HBK 儲存服務核心相依性注入輔助類別
    /// </summary>
    public static class HBKStorageCoreDependencyInjectionExtensions
    {
        /// <summary>
        /// 自動加入完整 HBK 儲存服務相關資源
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHBKStorageService(this IServiceCollection services)
        {
            services.AddScoped<StorageService>();
            services.AddScoped<StorageProviderService>();
            services.AddScoped<StorageGroupService>();
            services.AddScoped<FileEntityService>();
            services.AddScoped<FileSystemFactory>();
            return services;
        }
    }
}
