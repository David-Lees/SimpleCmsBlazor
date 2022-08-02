using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleCmsBlazor;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;
using MatBlazor;
using Plk.Blazor.DragDrop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = builder.Configuration.GetValue<string>("ApiUrl");
var storageUrl = builder.Configuration.GetValue<string>("StorageUrl");

builder.Services.AddHttpClient(HttpClients.Storage, client => client.BaseAddress = new Uri(storageUrl))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient(HttpClients.Api, client => client.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient(HttpClients.Public, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
});

builder.Services.AddScoped<IBrowserResizeService, BrowserResizeService>();
builder.Services.AddScoped<IBlobUploadService, BlobUploadService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<IImageService, ImageService>();   
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<ISiteService, SiteService>();

builder.Services.AddMatToaster(config =>
{
    config.Position = MatToastPosition.BottomRight;
    config.PreventDuplicates = true;
    config.NewestOnTop = true;
    config.ShowCloseButton = true;
    config.MaximumOpacity = 100;
    config.VisibleStateDuration = 5000;
    config.HideTransitionDuration = 500;
    config.ShowTransitionDuration = 500;
    config.ShowProgressBar = true;
});

builder.Services.AddBlazorDragDrop();

await builder.Build().RunAsync();
