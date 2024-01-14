using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Services;

public interface IBlobUploadService
{
    Task<string> GetUserDelegationKeyAsync();
}

public class BlobUploadService(IHttpClientFactory clientFactory) : IBlobUploadService
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient(HttpClients.Api);

    public async Task<string> GetUserDelegationKeyAsync()
    {
        var content = new StringContent(string.Empty);
        content.Headers.Add("x-ms-version", "2017-11-09");
        var response = await _httpClient.PostAsync($"/api/GetSasToken", content);
        return await response.Content.ReadAsStringAsync();
    }
}