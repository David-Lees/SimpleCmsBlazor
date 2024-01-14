using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;
using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleCmsBlazor.Shared;

public enum UploadStatus
{
    Pending,
    Uploading,
    Success,
    Failure
}

public class FileToUpload(IBrowserFile file)
{
    public long Progress { get; set; } = 0;
    public string Description { get; set; } = file.Name;
    public string ContentType { get; set; } = file.ContentType;
    public IBrowserFile File { get; set; } = file;
    public UploadStatus Status { get; set; } = UploadStatus.Pending;

    public CancellationTokenSource TokenSource { get; set; } = new();

    public void CancelUpload(MouseEventArgs _) => TokenSource.Cancel();

    public long UploadedBytes { get; set; } = 0;
}

public partial class FileUpload
{
    private readonly ConcurrentBag<FileToUpload> _filesToUpload = [];

    private string _hoverClass = string.Empty;

    public ElementReference FileDropContainer { get; set; }

    [Inject]
    public IFileUploadService FileUploadService { get; set; } = default!;

    [Parameter]
    public EventCallback<bool> UploadComplete { get; set; }

    [Parameter]
    public Guid ParentId { get; set; }

    /// <summary>
    /// Always clear the fil collection on each change.
    /// Allowing cumulative addition to collection before upload causes exception on upload.
    /// https://learn.microsoft.com/en-us/aspnet/core/blazor/file-uploads?view=aspnetcore-7.0&pivots=server
    /// </summary>
    /// <param name="e"></param>
    public void OnChange(InputFileChangeEventArgs e)
    {
        _filesToUpload.Clear();
        var files = e.GetMultipleFiles(100).Select(x => new FileToUpload(x)).ToList();
        foreach (var file in files)
        {
            _filesToUpload.Add(file);
        }
        StateHasChanged();
    }

    public async Task UploadFileQueue()
    {
        // Commence uploading all files simultaniously.
        await Parallel.ForEachAsync(_filesToUpload.Where(x => x.Status == UploadStatus.Pending), async (file, token) =>
        {
            try
            {
                await UploadChunks(file);
                file.Status = UploadStatus.Success;
            }
            catch
            {
                file.Status = UploadStatus.Failure;
            }
        });
    }

    private async Task<string> UploadChunk(Stream s, long bufferSize, Guid fileId, string filename)
    {
        var blockId = EncodeBase64String(Guid.NewGuid());
        var buffer = new byte[bufferSize];
        await s.ReadAsync(buffer);
        await FileUploadService.UploadFileChunk(new FileChunkDto
        {
            ParentId = ParentId,
            FileId = fileId,
            Name = filename,
            BlockId = blockId,
            Data = buffer
        });
        return blockId;
    }

    private async Task UploadChunks(FileToUpload file)
    {
        try
        {
            file.Status = UploadStatus.Uploading;

            await InvokeAsync(StateHasChanged);

            var fileId = Guid.NewGuid();
            var totalBytes = file.File.Size;
            long chunkSize = 1024 * 100; // 100kb chunk size
            long numChunks = totalBytes / chunkSize;
            long remainder = totalBytes % chunkSize;

            // Get file name
            string nameOnly = Path.GetFileNameWithoutExtension(file.File.Name);
            var extension = Path.GetExtension(file.File.Name);
            string newFileNameWithoutPath = $"{nameOnly}{extension}";

            // Store Ids of generated chunks
            var chunkList = new List<string>();

            using var inStream = file.File.OpenReadStream(long.MaxValue);

            // Upload chunks
            for (int i = 0; i < numChunks; i++)
            {
                chunkList.Add(await UploadChunk(inStream, chunkSize, fileId, newFileNameWithoutPath));
                file.UploadedBytes += chunkSize;
                file.Progress = 100 * file.UploadedBytes / totalBytes;
                await InvokeAsync(StateHasChanged);
            }
            if (remainder > 0)
            {
                chunkList.Add(await UploadChunk(inStream, remainder, fileId, newFileNameWithoutPath));
                file.UploadedBytes += remainder;
                file.Progress = 100 * file.UploadedBytes / totalBytes;
                await InvokeAsync(StateHasChanged);
            }

            // Tell the server that we have finished uploading chunks and the list of all chunks sent
            await FileUploadService.FinaliseUpload(new FileChunkListDto
            {
                ContentType = string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType,
                Description = file.Description,
                Size = file.UploadedBytes,
                ParentId = ParentId,
                BlockIds = chunkList,
                FileId = fileId,
                Name = newFileNameWithoutPath
            });

            // Make sure we end on 100% and don't have rounding errors leaving us at 99%
            file.Progress = 100;
            file.Status = UploadStatus.Success;
            await InvokeAsync(StateHasChanged);

            // If there are any files and all are now at status success, signal that uploading is complete
            if (!_filesToUpload.IsEmpty && _filesToUpload.All(x => x.Status == UploadStatus.Success))
            {
                await UploadComplete.InvokeAsync(true);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.ToString());
            file.Status = UploadStatus.Failure;
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OnDragEnter(DragEventArgs e) => _hoverClass = "hover";

    private void OnDragLeave(DragEventArgs e) => _hoverClass = string.Empty;

    private static string EncodeBase64String(Guid guid)
    {
        Span<byte> guidBytes = stackalloc byte[16];
        Span<byte> encodedBytes = stackalloc byte[24];

        MemoryMarshal.TryWrite(guidBytes, in guid);
        Base64.EncodeToUtf8(guidBytes, encodedBytes, out _, out _);

        return Encoding.UTF8.GetString(encodedBytes);
    }
}