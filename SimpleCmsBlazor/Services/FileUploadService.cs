using SimpleCmsBlazor.Models;
using System.Net.Http.Json;

namespace SimpleCmsBlazor.Services;

public interface IFileUploadService
{
    Task FinaliseUpload(FileChunkListDto chunks);

    Task UploadFileChunk(FileChunkDto chunk);
}

public class FileUploadService(IHttpClientFactory factory) : IFileUploadService
{
    private readonly HttpClient _http = factory.CreateClient(HttpClients.Api);

    public async Task UploadFileChunk(FileChunkDto chunk)
    {
        // Post as form data instead of JSON.
        // Data would get base64 encoded as JSON which would potentially double the size
        using var content = new MultipartFormDataContent
        {
            { new StringContent(chunk.FileId.ToString()), "fileId" },
            { new StringContent(chunk.Name), "name" },
            { new StringContent(chunk.BlockId), "blockId" },
            { new StringContent(chunk.ParentId.ToString()), "parentId" },
            { new StreamContent(new MemoryStream(chunk.Data)), "data", chunk.Name }
        };
        var response = await _http.PostAsync($"/api/chunk/{chunk.FileId}", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task FinaliseUpload(FileChunkListDto chunks)
    {
        // We need to tell the server that all chunks have been uplaoded
        var response = await _http.PostAsJsonAsync($"/api/finalise/{chunks.FileId}", chunks);
        response.EnsureSuccessStatusCode();
    }
}