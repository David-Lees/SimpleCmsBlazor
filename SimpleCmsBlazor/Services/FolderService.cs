using SimpleCmsBlazor.Models;
using System.Net.Http.Json;

namespace SimpleCmsBlazor.Services;

public interface IFolderService
{
    Task CreateAsync(GalleryFolder folder);
    Task DeleteAsync(GalleryFolder folder);
    Task<List<GalleryFolder>> GetFoldersAsync(bool reload = false);
    Task<List<GalleryImage>> GetImagesAsync(GalleryFolder folder);
    Task MoveAsync(GalleryFolder folder, Guid destination);
    Task ReloadAsync();
    void Update(List<GalleryFolder> galleryFolders);
}

public class FolderService : IFolderService
{
    private List<GalleryFolder>? _folders;
    private readonly HttpClient _client;

    public FolderService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient(HttpClients.Api);
    }

    public async Task<List<GalleryFolder>> GetFoldersAsync(bool reload = false)
    {
        if (_folders != null && !reload)
        {
            return _folders;
        }
        else
        {
            await ReloadAsync();
            return _folders!;
        }
    }

    public async Task ReloadAsync()
    {
        _folders = await _client.GetFromJsonAsync<List<GalleryFolder>>("/api/folder") ?? new();
        _folders.Sort((a, b) => a.Name.CompareTo(b.Name));
    }

    public void Update(List<GalleryFolder> galleryFolders)
    {
        _folders = galleryFolders;
    }

    public async Task DeleteAsync(GalleryFolder folder)
    {
        var request = new HttpRequestMessage
        {
            Content = JsonContent.Create(folder),
            Method = HttpMethod.Delete,
            RequestUri = new Uri("/api/folder")
        };
        await _client.SendAsync(request);
        await ReloadAsync();
    }

    public async Task MoveAsync(GalleryFolder folder, Guid destination)
    {
        await _client.PutAsJsonAsync($"/api/folder/{destination}", folder);
        await ReloadAsync();
    }

    public async Task CreateAsync(GalleryFolder folder)
    {
        await _client.PostAsJsonAsync("/api/folder", folder);
        await ReloadAsync();
    }

    public async Task<List<GalleryImage>> GetImagesAsync(GalleryFolder folder)
    {
        return await _client.GetFromJsonAsync<List<GalleryImage>>($"/api/folder/{folder.RowKey}") ?? new();
    }
}
