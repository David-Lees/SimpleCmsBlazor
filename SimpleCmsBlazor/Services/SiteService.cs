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

public class SiteService(IHttpClientFactory clientFactory, IHxMessengerService toastService) : ISiteService
{
    private Site? _site;
    private readonly HttpClient _storageClient = clientFactory.CreateClient(HttpClients.Storage);
    private readonly HttpClient _apiClient = clientFactory.CreateClient(HttpClients.Api);
    private readonly IHxMessengerService _toastService = toastService;

    public async Task<Site> GetSiteAsync()
    {
        _site ??= await _storageClient.GetFromJsonAsync<Site>($"/images/site.json?v={DateTime.UtcNow.Ticks}") ?? new();
        return _site;
    }

    public async Task SaveSiteAsync(Site site)
    {
        var response = await _apiClient.PostAsJsonAsync("/api/UpdateSite", site);
        if (response.IsSuccessStatusCode)
        {
            _site = await _storageClient.GetFromJsonAsync<Site>($"/images/site.json?v={DateTime.UtcNow.Ticks}") ?? site;
            _toastService.AddInformation("Site Saved");
        }
        else
        {
            _toastService.AddError($"Unable to save site. {response.ReasonPhrase}");
        }
    }
}