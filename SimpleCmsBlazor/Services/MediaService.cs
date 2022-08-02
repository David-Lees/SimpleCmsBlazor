﻿namespace SimpleCmsBlazor.Services;

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MatBlazor;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using SimpleCmsBlazor.Models;
using System.Net.Http.Json;

public interface IMediaService
{
    Task Delete(GalleryImage image);
    Task<List<GalleryImage>> LoadAsync(GalleryFolder folder);
    Task Move(GalleryImage image, GalleryFolder folder);
    Task Upload(List<FileUploadProgress> files);
}

public class FileUploadProgress : IProgress<long>
{
    public long Progress { get; set; } = 0;
    public IBrowserFile File { get; private set; }

    public FileUploadProgress(IBrowserFile file, Action action)
    {
        File = file;
        ProgressMade = action;
    }

    public CancellationTokenSource TokenSource { get; set; } = new();

    public void Report(long value)
    {
        Progress = value / File.Size;
        ProgressMade();
    }

    public Action ProgressMade { get; set; }

    public void CancelUpload(MouseEventArgs _) => TokenSource.Cancel();
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

    public async Task Upload(List<FileUploadProgress> files)
    {
        var key = await _blobUploadService.GetUserDelegationKeyAsync();
        var credentials = new AzureSasCredential(key);
        var storageUrl = _configuration.GetValue<string>("storageUrl");

        await Parallel.ForEachAsync(files, async (file, token) =>
        {
            token.Register(() => file.TokenSource.Cancel());
            var uri = new Uri($"{storageUrl}/image-upload/{file.File.Name}");
            try
            {
                var blobClient = new BlobClient(uri, credentials);
                await blobClient.UploadAsync(file.File.OpenReadStream(cancellationToken: file.TokenSource.Token), new BlobUploadOptions
                {
                    ProgressHandler = file
                }, file.TokenSource.Token);
                _toastService.Add($"{file.File.Name} uploaded", MatToastType.Success);
            }
            catch (RequestFailedException ex)
            {
                _toastService.Add(ex.Message, MatToastType.Danger);
            }
        });
    }
}
