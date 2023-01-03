using SimpleCmsBlazor.Models;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace SimpleCmsBlazor.Pages;

public partial class ImageEdit
{
    private readonly HttpClient _storageClient;
    private SKBitmap? _source;
    private SKBitmap? _destination;
    private SKGLView _canvas = default!;

    public ImageEdit(IHttpClientFactory clientFactory)
    {
        _storageClient = clientFactory.CreateClient(HttpClients.Storage);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    public async Task LoadImageAsync(string url)
    {
        var image = await _storageClient.GetByteArrayAsync(url);
        _source = SKBitmap.Decode(image);
    }

    public void OnPaintSurface(SKPaintGLSurfaceEventArgs args)
    {
        var canvas = args.Surface.Canvas;
        canvas.Clear(SKColors.Wheat);
        if (_source == null) return;

        canvas.DrawBitmap(_source, new SKRect(0, 0, 800, 600));
    }
}
