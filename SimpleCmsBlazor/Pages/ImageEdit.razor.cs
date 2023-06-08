using Microsoft.AspNetCore.Components.Web;
using SimpleCmsBlazor.Models;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleCmsBlazor.Pages;

public class Marker
{
    public double X { get; set; }
    public double Y { get; set; }

    public Marker(double x, double y)
    {
        X = x;
        Y = y;
    }

    public SKPoint ToPoint()
    {
        return new SKPoint((float)(X), (float)(Y));
    }
}

public partial class ImageEdit
{
    private readonly HttpClient _storageClient;
    private readonly HttpClient _client;
    private SKBitmap? _source;
    private SKBitmap? _destination;
    private SKGLView _canvas = default!;
    private double _canvasWidth = 800;

    public ImageEdit(IHttpClientFactory clientFactory)
    {
        _storageClient = clientFactory.CreateClient(HttpClients.Storage);
        _client = clientFactory.CreateClient(HttpClients.Public);
        _canvasWidth = WindowWidth();
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadImageAsync();
    }

    public async Task LoadImageAsync()
    {
        //var image = await _storageClient.GetByteArrayAsync(url);
        var image = await _client.GetByteArrayAsync("/test.jpg");
        _source = SKBitmap.Decode(image);
        _destination = SKBitmap.Decode(image);
    }

    public void OnPaintSurface(SKPaintGLSurfaceEventArgs args)
    {
        var canvas = args.Surface.Canvas;
        canvas.Clear(SKColors.Wheat);
        if (_source == null) return;

        canvas.DrawBitmap(_source, new SKRect(0, 0, 800, 600));

        if (_destination == null) return;
        canvas.DrawBitmap(_destination, new SKRect(800, 0, 1600, 600));
    }


    [JSImport("globalThis.window.GetWindowWidth")]
    public static partial double WindowWidth();

    public List<Marker> Markers { get; set; } = new()
    {
        new Marker(257,72),
        new Marker(279,543),
        new Marker(577,533),
        new Marker(639,90)
    };

    public static void DragEnd(Marker marker, DragEventArgs eventArgs)
    {
        Console.WriteLine(JsonSerializer.Serialize(eventArgs));
        marker.X = eventArgs.ClientX;
        marker.Y = eventArgs.ClientY;
    }

    public void ProcessImage()
    {
        if (_source == null) return;
        var canvas = new SKCanvas(_destination);
        canvas.Clear();
        var matrix = CreateMatrixFromPoints(Markers[0].ToPoint(), Markers[3].ToPoint(), Markers[2].ToPoint(), Markers[1].ToPoint(), _source.Width, _source.Height);
        canvas.SetMatrix(matrix);
        canvas.DrawBitmap(_source, 0, 0);
    }

    public static SKMatrix CreateMatrixFromPoints(SKPoint topLeft, SKPoint topRight, SKPoint botRight, SKPoint botLeft, float width, float height)
    {
        (float x1, float y1) = (-topLeft.X / 800 * width, -topLeft.Y / 600 * height);
        (float x2, float y2) = ((width - topRight.X + width) / 800 * width, -topRight.Y / 600 * height);
        (float x3, float y3) = ((width - botRight.X + width) / 800 * width, (height - botRight.Y + height) / 600 * height);
        (float x4, float y4) = (-botLeft.X / 800 * width, (height - botLeft.Y + height) / 600 * height);
        (float w, float h) = (width, height);

        float scaleX = (y1 * x2 * x4 - x1 * y2 * x4 + x1 * y3 * x4 - x2 * y3 * x4 - y1 * x2 * x3 + x1 * y2 * x3 - x1 * y4 * x3 + x2 * y4 * x3) / (x2 * y3 * w + y2 * x4 * w - y3 * x4 * w - x2 * y4 * w - y2 * w * x3 + y4 * w * x3);
        float skewX = (-x1 * x2 * y3 - y1 * x2 * x4 + x2 * y3 * x4 + x1 * x2 * y4 + x1 * y2 * x3 + y1 * x4 * x3 - y2 * x4 * x3 - x1 * y4 * x3) / (x2 * y3 * h + y2 * x4 * h - y3 * x4 * h - x2 * y4 * h - y2 * h * x3 + y4 * h * x3);
        float transX = x1;
        float skewY = (-y1 * x2 * y3 + x1 * y2 * y3 + y1 * y3 * x4 - y2 * y3 * x4 + y1 * x2 * y4 - x1 * y2 * y4 - y1 * y4 * x3 + y2 * y4 * x3) / (x2 * y3 * w + y2 * x4 * w - y3 * x4 * w - x2 * y4 * w - y2 * w * x3 + y4 * w * x3);
        float scaleY = (-y1 * x2 * y3 - y1 * y2 * x4 + y1 * y3 * x4 + x1 * y2 * y4 - x1 * y3 * y4 + x2 * y3 * y4 + y1 * y2 * x3 - y2 * y4 * x3) / (x2 * y3 * h + y2 * x4 * h - y3 * x4 * h - x2 * y4 * h - y2 * h * x3 + y4 * h * x3);
        float transY = y1;
        float persp0 = (x1 * y3 - x2 * y3 + y1 * x4 - y2 * x4 - x1 * y4 + x2 * y4 - y1 * x3 + y2 * x3) / (x2 * y3 * w + y2 * x4 * w - y3 * x4 * w - x2 * y4 * w - y2 * w * x3 + y4 * w * x3);
        float persp1 = (-y1 * x2 + x1 * y2 - x1 * y3 - y2 * x4 + y3 * x4 + x2 * y4 + y1 * x3 - y4 * x3) / (x2 * y3 * h + y2 * x4 * h - y3 * x4 * h - x2 * y4 * h - y2 * h * x3 + y4 * h * x3);
        float persp2 = 1;

        Console.WriteLine($"{scaleX}, {skewX}, {transX}, {skewY}, {scaleY}, {transY}, {persp0}, {persp1}, {persp2}");
        return new SKMatrix(scaleX / 10, skewX, transX / 10, skewY, scaleY / 10, transY / 10, persp0, persp1, persp2);
    }
}
