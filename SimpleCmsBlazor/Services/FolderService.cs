using SimpleCmsBlazor.Models;
using System.Net.Http.Json;
using System.Reactive.Subjects;

namespace SimpleCmsBlazor.Services;

public class FolderService(IHttpClientFactory clientFactory)
{
    private int count = 0;
    private List<GalleryFolder>? _folders;
    private readonly HttpClient _client = clientFactory.CreateClient(HttpClients.Api);

    private readonly Subject<int> _reloadSubject = new();

    public IObservable<int> ReloadSubscription => _reloadSubject;

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
        _folders = await _client.GetFromJsonAsync<List<GalleryFolder>>("/api/folder") ?? [];
        _folders.Sort((a, b) => a.Name.CompareTo(b.Name));
        _reloadSubject.OnNext(count++);
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
        return await _client.GetFromJsonAsync<List<GalleryImage>>($"/api/folder/{folder.RowKey}") ?? [];
    }
}