using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using OpenCvSharp;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.OpenCVSharp4;
using SimpleCmsBlazor.Services;
using SpawnDev.BlazorJS.JSObjects;
using System.Runtime.InteropServices.JavaScript;

namespace SimpleCmsBlazor.Pages;

public partial class AdminImageEdit
{
    [Parameter]
    public Guid PartitionKey { get; set; }

    [Parameter]
    public Guid RowKey { get; set; }

    [Inject]
    public FolderService FolderService { get; set; } = default!;

    [Inject]
    public IMediaService MediaService { get; set; } = default!;

    [Inject]
    public IConfiguration? Config { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    public List<Marker> Markers { get; set; } = [];

    public GalleryImage? Original { get; set; }

    private ElementReference canvasSrcRef;
    private ElementReference canvasDestRef;
    private readonly Mat Src = new();
    private Mat? Dest;

    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }
    public int CanvasX { get; set; }
    public int CanvasY { get; set; }

    public int ScaledWidth { get; set; }
    public int ScaledHeight { get; set; }

    public int OffsetX { get; set; }
    public int OffsetY { get; set; }

    public double Ratio { get; set; }

    private double contrast = 1;
    private int brightness = 0;
    private double saturation = 1;

    protected override void OnInitialized()
    {
        Markers.AddRange(
        [
            new(0, 0),
            new(0, 0),
            new(0, 0),
            new(0, 0),
        ]);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        using var canvasSrcEl = new HTMLCanvasElement(canvasSrcRef);
        using var canvasSrcCtx = canvasSrcEl.Get2DContext();

        if (firstRender)
        {
            CanvasWidth = (int)canvasSrcEl.OffsetWidth;
            CanvasHeight = (int)canvasSrcEl.OffsetHeight;

            var rect = await JSRuntime.InvokeAsync<BoundingClientRect>("window.GetRect", canvasSrcRef);
            CanvasX = (int)rect.X;
            CanvasY = (int)rect.Y;

            var folders = await FolderService.GetFoldersAsync();
            var folder = folders.Find(x => x.RowKey == PartitionKey) ?? throw new NotFoundException();
            var images = await FolderService.GetImagesAsync(folder) ?? throw new NotFoundException();
            Original = images.Find(x => x.RowKey == RowKey) ?? throw new NotFoundException();
            await Src.LoadImageURL(GetImagePath(Original, string.IsNullOrEmpty(Original.OriginalPath) ? "Raw" : "Original") ?? throw new NotFoundException());

            SetScaled(canvasSrcCtx);

            Markers[0].X = OffsetX;
            Markers[0].Y = OffsetY;
            Markers[1].X = OffsetX + ScaledWidth;
            Markers[1].Y = OffsetY;
            Markers[3].X = OffsetX + ScaledWidth;
            Markers[3].Y = OffsetY + ScaledHeight;
            Markers[2].X = OffsetX;
            Markers[2].Y = OffsetY + ScaledHeight;

            DrawOnCanvas(Src, canvasSrcCtx);
            await InvokeAsync(StateHasChanged);
        }
    }

    public void SetScaled(CanvasRenderingContext2D ctx)
    {
        var hRatio = ctx.Canvas.OffsetWidth / (double)Src.Width;
        var vRatio = ctx.Canvas.OffsetHeight / (double)Src.Height;
        Ratio = Math.Min(hRatio, vRatio);
        ScaledWidth = (int)(Src.Width * Ratio);
        ScaledHeight = (int)(Src.Height * Ratio);
        OffsetX = (int)((ctx.Canvas.OffsetWidth - ScaledWidth) / 2);
        OffsetY = (int)((ctx.Canvas.OffsetHeight - ScaledHeight) / 2);
    }

    public void DrawOnCanvas(Mat mat, CanvasRenderingContext2D ctx)
    {
        ctx.Canvas.Width = (int)ctx.Canvas.OffsetWidth;
        ctx.Canvas.Height = (int)ctx.Canvas.OffsetHeight;

        ctx.FillStyle = "Linen";
        ctx.FillRect(0, 0, (int)ctx.Canvas.OffsetWidth, (int)ctx.Canvas.OffsetHeight);

        var hRatio = ctx.Canvas.OffsetWidth / (double)mat.Width;
        var vRatio = ctx.Canvas.OffsetHeight / (double)mat.Height;
        var ratio = Math.Min(hRatio, vRatio);

        var size = new Size((int)(mat.Width * ratio), (int)(mat.Height * ratio));
        var output = new Mat(size, mat.Type());
        Cv2.Resize(mat, output, size);

        var bytes = output.GetRGBABytes();
        if (bytes != null)
        {
            ctx.PutImageBytes(bytes, output.Width, output.Height, OffsetX, OffsetY);
        }
    }

    private string GetImagePath(GalleryImage image, string quality)
    {
        return $"{Config?.GetValue<string>("StorageUrl") ?? string.Empty}/images/{image.GetPropValue<string>(quality + "Path")}";
    }

    public void ProcessImage()
    {
        var markers = Markers.Select(m => new Point2f((float)((m.X - OffsetX) / Ratio), (float)((m.Y - OffsetY) / Ratio))).ToList();
        var h = (Distance(markers[0], markers[2]) + Distance(markers[1], markers[3])) / 2;
        var w = (Distance(markers[0], markers[1]) + Distance(markers[2], markers[3])) / 2;

        using var matrix = Cv2.GetPerspectiveTransform([markers[0], markers[1], markers[3], markers[2]], [new(0, 0), new(w, 0), new(w, h), new(0, h)]);

        var size = new Size(w, h);
        Dest = new Mat(size, Src.Type());

        using var canvasDestEl = new HTMLCanvasElement(canvasDestRef);
        using var canvasDestCtx = canvasDestEl.Get2DContext();

        Cv2.WarpPerspective(Src, Dest, matrix, size);

        // Britghtness and contrast
        Cv2.ConvertScaleAbs(Dest, Dest, contrast, brightness);

        // Saturation
        var imghsv = new Mat(size, MatType.CV_8UC1);
        Cv2.CvtColor(Dest, imghsv, ColorConversionCodes.RGB2HSV);
        Cv2.Split(imghsv, out Mat[] channels);
        channels[1] = channels[1].Multiply(saturation);
        Cv2.Merge(channels, imghsv);
        Cv2.CvtColor(imghsv, Dest, ColorConversionCodes.HSV2BGR);

        DrawOnCanvas(Dest, canvasDestCtx);
    }

    public async Task SaveImage()
    {
        if (Original != null && Dest != null)
        {
            await MediaService.SaveEdit(Original, Dest);
        }
    }

    private static float Distance(Point2f a, Point2f b)
    {
        var c = b.X - a.X;
        var d = b.Y - a.Y;
        return MathF.Sqrt(c * c + d * d);
    }

    private int pos3 = 0;
    private int pos4 = 0;
    private bool dragging = false;
    private Marker? dragMarker;

    public void StartDrag(Marker m)
    {
        dragMarker = m;
        dragging = true;
    }

    [JSImport("globalThis.window.GetY")]
    private static partial int GetScroll();

    public void HandleDrag(MouseEventArgs e)
    {
        if (dragging && dragMarker != null)
        {
            pos3 = (int)e.ClientX;
            pos4 = (int)e.ClientY;

            dragMarker.X = pos3 - CanvasX;
            dragMarker.Y = pos4 - CanvasY + GetScroll();
            StateHasChanged();
        }
    }

    public void CloseDrag()
    {
        dragging = false;
    }
}