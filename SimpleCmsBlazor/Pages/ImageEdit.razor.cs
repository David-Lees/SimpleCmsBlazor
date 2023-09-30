using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using OpenCvSharp;
using SimpleCmsBlazor.Models;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

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

    public Point2f ToPoint2f(double factorx, double factory)
    {
        return new Point2f((float)(X * factorx), (float)(Y * factory));
    }
}

public partial class ImageEdit : IDisposable
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private readonly HttpClient _storageClient;
    private readonly HttpClient _client;
    private Mat? _source;
    private Mat? _destination;

    private ElementReference srcCanvasElement;
    private ElementReference dstCanvasElement;
    private CanvasClient? srcCanvasClient;
    private CanvasClient? dstCanvasClient;
    private bool disposedValue;

    public ImageEdit(IHttpClientFactory clientFactory)
    {
        _storageClient = clientFactory.CreateClient(HttpClients.Storage);
        _client = clientFactory.CreateClient(HttpClients.Public);
    }

    //protected override async Task OnInitializedAsync()
    //{
    //    await LoadImageAsync();
    //}

    public async Task LoadImageAsync()
    {
        //var image = await _storageClient.GetByteArrayAsync(url);
        var image = await _client.GetByteArrayAsync("/test.jpg");
        _source = Mat.FromImageData(image, ImreadModes.Unchanged);
        _destination = _source.Clone();
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        srcCanvasClient ??= new CanvasClient(JSRuntime, srcCanvasElement);
        dstCanvasClient ??= new CanvasClient(JSRuntime, dstCanvasElement);

        if (_source is not null)
        {
            var srcMat = _source.Clone().Resize(new Size(800, 600));
            await srcCanvasClient.DrawMatAsync(srcMat);
        }
        if (_destination is not null)
        {
            var dstMat = _destination.Clone().Resize(new Size(800, 600));
            await dstCanvasClient.DrawMatAsync(dstMat);
        }
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
        if (_source == null || _destination == null) return;

        var fx = 1 / 800 * _source.Width;
        var fy = 1 / 600 * _source.Height;

        var size = new Size(_source.Width, _source.Height);

        using var matrix = Cv2.GetPerspectiveTransform(
            new Point2f[] { Markers[0].ToPoint2f(fx, fy), Markers[3].ToPoint2f(fx, fy), Markers[2].ToPoint2f(fx, fy), Markers[1].ToPoint2f(fx, fy) },
            new Point2f[] { new(0, 0), new(_source.Width, 0), new(_source.Width, _source.Height), new Point2f(0, _source.Height) }
        );

        Cv2.WarpPerspective(_source, _destination, matrix, size);
        StateHasChanged();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null

            _source?.Dispose();
            _destination?.Dispose();
            disposedValue = true;
        }
    }

    ~ImageEdit()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
