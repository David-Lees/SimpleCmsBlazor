using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using OpenCvSharp;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.OpenCVSharp4;
using System.Net.Http.Json;

namespace SimpleCmsBlazor.Services;

public interface IMediaService
{
    Task Delete(GalleryImage image);

    Task<List<GalleryImage>> LoadAsync(GalleryFolder folder);

    Task Move(GalleryImage image, GalleryFolder folder);

    Task SaveEdit(GalleryImage image, Mat data);
}

public class FileUploadProgress(IBrowserFile file, Action action) : IProgress<long>
{
    public long Progress { get; set; } = 0;
    public IBrowserFile File { get; private set; } = file;

    public CancellationTokenSource TokenSource { get; set; } = new();

    public void Report(long value)
    {
        Progress = value / File.Size;
        ProgressMade();
    }

    public Action ProgressMade { get; set; } = action;

    public void CancelUpload(MouseEventArgs _) => TokenSource.Cancel();
}

public class MediaService(IHttpClientFactory clientFactory, IHxMessengerService toastService) : IMediaService
{
    private readonly HttpClient _apiClient = clientFactory.CreateClient(HttpClients.Api);
    private readonly IHxMessengerService _toastService = toastService;

    public async Task<List<GalleryImage>> LoadAsync(GalleryFolder folder)
    {
        var content = new StringContent(string.Empty);
        content.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate, post-check=0, pre-check=0");
        content.Headers.Add("Pragma", "no-cache");
        content.Headers.Add("Expires", "0");
        return await _apiClient.GetFromJsonAsync<List<GalleryImage>>($"$/api/folder/${folder.RowKey}") ?? [];
    }

    public async Task Delete(GalleryImage image)
    {
        var response = await _apiClient.DeleteAsync($"/api/folder/{image.PartitionKey}/{image.RowKey}");
        if (response.IsSuccessStatusCode)
        {
            _toastService.AddInformation("Image deleted");
        }
        else
        {
            _toastService.AddError("Unable to delete image");
        }
    }

    public async Task Move(GalleryImage image, GalleryFolder folder)
    {
        var response = await _apiClient.DeleteAsync($"/api/MoveImage?oldParent={image.PartitionKey}&newParent={folder.RowKey}&id={image.RowKey}");
        if (response.IsSuccessStatusCode)
        {
            _toastService.AddInformation("Image moved");
        }
        else
        {
            _toastService.AddError("Unable to move image");
        }
    }

    public async Task SaveEdit(GalleryImage image, Mat data)
    {
        var bytes = data.GetRGBABytes();
        var response = await _apiClient.PostAsync(
            $"/api/SaveEdit?rowKey={image.RowKey}&partitionKey={image.PartitionKey}&width={data.Width}&height={data.Height}",
            new ByteArrayContent(bytes));
        if (response.IsSuccessStatusCode)
        {
            _toastService.AddInformation("Image saved");
        }
        else
        {
            _toastService.AddError("Unable to save image");
        }
    }
}