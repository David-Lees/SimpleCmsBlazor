namespace SimpleCmsBlazor.Services;

using Azure;
using Azure.Storage.Blobs;
using MatBlazor;
using Microsoft.AspNetCore.Components.Forms;
using SimpleCmsBlazor.Models;
using System.Net.Http.Json;

public interface IMediaService
{
    Task Delete(GalleryImage image);
    Task<List<GalleryImage>> LoadAsync(GalleryFolder folder);
    Task Move(GalleryImage image, GalleryFolder folder);
    Task Upload(InputFileChangeEventArgs e);
}

public class MediaService : IMediaService
{
    private readonly HttpClient _apiClient;
    private readonly BlobUploadService _blobUploadService;
    private readonly IMatToaster _toastService;
    private readonly IConfiguration _configuration;

    public MediaService(
        IHttpClientFactory clientFactory,
        BlobUploadService blobUploadService,
        IMatToaster toastService,
        IConfiguration configuration)
    {
        _apiClient = clientFactory.CreateClient(HttpClients.Api);
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
        return await _apiClient.GetFromJsonAsync<List<GalleryImage>>($"$/api/folder/${folder.RowKey}") ?? new();
    }


    public async Task Delete(GalleryImage image)
    {
        var response = await _apiClient.DeleteAsync($"/api/folder/{image.PartitionKey}/{image.RowKey}");
        if (response.IsSuccessStatusCode)
        {
            _toastService.Add("Image deleted", MatToastType.Success);
        }
        else
        {
            _toastService.Add("Unable to delete image ", MatToastType.Danger);
        }
    }

    public async Task Move(GalleryImage image, GalleryFolder folder)
    {
        var response = await _apiClient.DeleteAsync($"/api/MoveImage?oldParent={image.PartitionKey}&newParent={folder.RowKey}&id={image.RowKey}");
        if (response.IsSuccessStatusCode)
        {
            _toastService.Add("Image moved", MatToastType.Success);
        }
        else
        {
            _toastService.Add("Unable to move image ", MatToastType.Danger);
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
                    _toastService.Add($"{file.Name} uploaded", MatToastType.Success);
                }
                catch (RequestFailedException ex)
                {
                    _toastService.Add(ex.Message, MatToastType.Danger);
                }
            }
        }
    }
}
