using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleCmsBlazor;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;
using Havit.Blazor.Components.Web;
using SpawnDev.BlazorJS;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// BlazorJS for easy JS interop
builder.Services.AddBlazorJSRuntime();

var apiUrl = builder.Configuration.GetValue<string>("ApiUrl") ?? string.Empty;
var storageUrl = builder.Configuration.GetValue<string>("StorageUrl") ?? string.Empty;

builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
builder.Services.AddHttpClient(HttpClients.Storage, client => client.BaseAddress = new Uri(storageUrl))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient(HttpClients.Api, client => client.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient(HttpClients.Public, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration.GetValue<string>("Scope") ?? string.Empty);
});

builder.Services.AddHxServices();
builder.Services.AddScoped<IBrowserResizeService, BrowserResizeService>();
builder.Services.AddScoped<IBlobUploadService, BlobUploadService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddSingleton<DragDropService>();
builder.Services.AddHxMessenger();
builder.Services.AddHxMessageBoxHost();

await builder.Build().RunAsync();