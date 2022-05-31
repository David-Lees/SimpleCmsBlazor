namespace SimpleCmsBlazor.Services;

using Azure;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;
using SimpleCmsBlazor.Models;
using System.Net.Http.Json;

public class MediaService
{
    private readonly HttpClient _client;
    private readonly BlobUploadService _blobUploadService;
    private readonly ToastService _toastService;
    private readonly IConfiguration _configuration;

    public MediaService(
        IHttpClientFactory clientFactory, 
        BlobUploadService blobUploadService, 
        ToastService toastService,
        IConfiguration configuration)
    {
        _client = clientFactory.CreateClient(HttpClients.Private);
        _blobUploadService = blobUploadService;
        _toastService = toastService;
        _configuration = configuration;
    }

    public async Task<List<GalleryImage>> LoadAsync(GalleryFolder folder)
    {
        var content = new StringContent(string.Empty);
        content.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate, post-check=0, pre-check=0");
        content.Headers.Add("Pragma", "no-cache");
        content.Headers.Add("Expires", "0");

        return await _client.GetFromJsonAsync<List<GalleryImage>>($"/api/folder/{folder.RowKey}") ?? new();
    }

    public async Task Delete(GalleryImage image)
    {
        var response = await _client.DeleteAsync($"/api/folder/{image.PartitionKey}/{image.RowKey}");
        if (response.IsSuccessStatusCode)
        {
            _toastService.Post(new() { Body = "Image deleted", State = ToastService.ToastState.Success });
        }
        else
        {
            _toastService.Post(new() { Body = "Unable to delete image ", State = ToastService.ToastState.Error });
        }
    }

    public async Task Move(GalleryImage image, GalleryFolder folder)
    {
        var response = await _client.DeleteAsync($"/api/MoveImage?oldParent={image.PartitionKey}&newParent={folder.RowKey}&id={image.RowKey}");
        if (response.IsSuccessStatusCode)
        {
            _toastService.Post(new() { Body = "Image moved", State = ToastService.ToastState.Success });
        }
        else
        {
            _toastService.Post(new() { Body = "Unable to move image ", State = ToastService.ToastState.Error });
        }
    }

    public async Task Upload(InputFileChangeEventArgs e)
    {
        var key = await _blobUploadService.GetUserDelegationKeyAsync();
        var credentials = new AzureSasCredential(key);

        if (e.FileCount > 0)
        {
            foreach (var file in e.GetMultipleFiles())
            {
                try
                {
                    var uri = new Uri($"{_configuration.GetValue<string>("storageUrl")}/image-upload/{file.Name}");
                    var blobClient = new BlobClient(uri, credentials);
                    await blobClient.UploadAsync(file.OpenReadStream());
                    _toastService.Post(new() { Body = $"{file.Name} uploaded", State = ToastService.ToastState.Success });
                }
                catch (RequestFailedException ex)
                {
                    _toastService.Post(new() { Body = ex.Message, State = ToastService.ToastState.Error });
                }
            }
        }
    }
}
