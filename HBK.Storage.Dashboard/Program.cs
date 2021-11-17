using Blazored.LocalStorage;
using Blazored.SessionStorage;
using HBK.Storage.Dashboard;
using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Models;
using HBK.Storage.Dashboard.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

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

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<StateContainer>();

builder.Services.AddScoped<HBKDialogService>();
builder.Services.AddScoped<HBKLayoutComponentService>();

builder.Services.AddScoped<HBKStorageApi>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var stateContainer = sp.GetRequiredService<StateContainer>();
    return new HBKStorageApi(config["HBKStorage:Url"], new HttpClient(), stateContainer);
});


await builder.Build().RunAsync();
