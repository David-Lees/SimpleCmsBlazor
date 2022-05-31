using SimpleCmsBlazor.Models;
using System.Net.Http.Json;

namespace SimpleCmsBlazor.Services;

public class FolderService
{
    private List<GalleryFolder>? _folders;
    private readonly HttpClient _client;

    public FolderService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient(HttpClients.Private);
    }

    public async Task<List<GalleryFolder>> GetFoldersAsync(bool reload = false)
    {
        if (_folders != null && !reload)
        {
            return _folders;
        }
        else
        {
            _folders = await _client.GetFromJsonAsync<List<GalleryFolder>>("/api/folder") ?? new();
            return _folders;
        }
    }
}
