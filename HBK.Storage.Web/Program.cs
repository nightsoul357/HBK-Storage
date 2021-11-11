using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Services;
using HBK.Storage.Web.Authentications;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.SessionStorage;
using HBK.Storage.Web.Containers;
using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.Features;
using Blazored.LocalStorage;
using HBK.Storage.Web.StorageServices;
using BlazorDownloadFile;
using System.Threading;

namespace HBK.Storage.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
            });

            builder.Services.AddScoped<AuthenticationStateProvider, HBKAuthStateProvider>();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<StateContainer>();

            builder.Services.AddScoped<StorageServiceProxy>(sp =>
            {
                var sessionStorageService = sp.GetRequiredService<ISyncSessionStorageService>();
                var localStorageService = sp.GetRequiredService<ISyncLocalStorageService>();
                return new StorageServiceProxy(new SessionStorageService(sessionStorageService),
                    new StorageServiceProxy(new LocalStorageService(localStorageService), null));
            });

            builder.Services.AddScoped<HBKStorageApi>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var stateContainer = sp.GetRequiredService<StateContainer>();
                return new HBKStorageApi(config["HBKStorage:Url"], new HttpClient() { Timeout = Timeout.InfiniteTimeSpan }, stateContainer);
            });

            builder.Services.AddScoped<HBKDialogService>();
            builder.Services.AddScoped<ClipboardService>();
            builder.Services.AddScoped<NavigationSerivce>();

            builder.Services.AddBlazorDownloadFile();

            await builder.Build().RunAsync();
        }
    }
}
