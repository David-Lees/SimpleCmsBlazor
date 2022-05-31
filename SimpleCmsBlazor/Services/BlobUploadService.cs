﻿using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Services;

public class BlobUploadService
{
    private readonly HttpClient _httpClient;

    public BlobUploadService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient(HttpClients.Private);
    }

    public async Task<string> GetUserDelegationKeyAsync()
    {
        var content = new StringContent(string.Empty);
        content.Headers.Add("x-ms-version", "2017-11-09");
        var response = await _httpClient.PostAsync($"/api/GetSasToken", content);
        return await response.Content.ReadAsStringAsync();
    }
}
