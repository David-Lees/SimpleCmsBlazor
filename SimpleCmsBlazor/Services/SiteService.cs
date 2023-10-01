using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using SimpleCmsBlazor.Models;
using System.Net.Http.Json;

namespace SimpleCmsBlazor.Services;

public interface ISiteService
{
    Task<Site> GetSiteAsync();

    Task SaveSiteAsync(Site site);
}

public class SiteService : ISiteService
{
    private Site? _site;
    private readonly HttpClient _storageClient;
    private readonly HttpClient _apiClient;
    private readonly IHxMessengerService _toastService;

    public SiteService(IHttpClientFactory clientFactory, IHxMessengerService toastService)
    {
        _storageClient = clientFactory.CreateClient(HttpClients.Storage);
        _apiClient = clientFactory.CreateClient(HttpClients.Api);
        _toastService = toastService;
    }

    public async Task<Site> GetSiteAsync()
    {
        _site ??= await _storageClient.GetFromJsonAsync<Site>("/images/site.json") ?? new();
        return _site;
    }

    public async Task SaveSiteAsync(Site site)
    {
        var response = await _apiClient.PostAsJsonAsync("/api/UpdateSite", site);
        if (response.IsSuccessStatusCode)
        {
            var newSite = await response.Content.ReadFromJsonAsync<Site>() ?? site;
            _site = newSite;
            _toastService.AddInformation("Site Saved");
        }
        else
        {
            _toastService.AddError($"Unable to save site. {response.ReasonPhrase}");
        }
    }
}