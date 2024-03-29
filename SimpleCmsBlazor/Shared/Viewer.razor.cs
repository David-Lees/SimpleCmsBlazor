using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SimpleCmsBlazor.Shared;

public partial class Viewer : ComponentBase, IDisposable
{
    [Inject]
    public ILogger<Viewer>? Log { get; set; }

    [Inject]
    public IImageService? ImageService { get; set; }

    [Inject]
    public IConfiguration? Config { get; set; }

    [Inject]
    public IBrowserResizeService? BrowserResizeService { get; set; }

    private bool showViewer;
    private string viewerClass = "viewer-fade-in";
    public bool LeftArrowVisible { get; set; } = true;
    public bool RightArrowVisible { get; set; } = true;
    private string categorySelected = "Raw";
    private decimal transform;
    private bool qualitySelectorShown = false;
    private string qualitySelected = "auto";
    private List<GalleryImage> images = [];
    private IDisposable? imagesUpdatedSubscription;
    private IDisposable? imageSelectedSubscription;
    private IDisposable? showImageSubscription;
    private IDisposable? resizeSubscription;
    private int currentIdx = 0;
    private decimal width;
    private decimal height;

    private sealed record TouchDataRecord(TouchPoint Point, DateTime Time);
    private TouchDataRecord? TouchData;

    protected override async Task OnInitializedAsync()
    {
        resizeSubscription = BrowserResizeService?.OnResize.Subscribe(async x =>
        {
            await OnResize(x.Item1, x.Item2);
        });
        imagesUpdatedSubscription = ImageService?.ImagesUpdated.Subscribe(x =>
        {
            images = x;
            StateHasChanged();
        });
        imageSelectedSubscription = ImageService?.ImagesSelectedIndexUpdated.Subscribe(newIndex =>
        {
            currentIdx = newIndex;
            foreach (var image in images)
            {
                image.Active = false;
            }
            images[currentIdx].Active = true;
            transform = 0;
            UpdateQuality();
            StateHasChanged();
        });
        showImageSubscription = ImageService?.ShowImageViewerChanged.Subscribe(x =>
        {
            showViewer = x;
            viewerClass = x ? "viewer-fade-in" : "viewer-fade-out";
            StateHasChanged();
        });
        width = await BrowserResizeService!.GetInnerWidth();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
        imagesUpdatedSubscription?.Dispose();
        imageSelectedSubscription?.Dispose();
        showImageSubscription?.Dispose();
        resizeSubscription?.Dispose();
    }

    public void TouchStart(TouchEventArgs touch)
    {
        TouchData = new TouchDataRecord(touch.TargetTouches[0], DateTime.Now);
    }

    public void TouchEnd(TouchEventArgs touch)
    {
        var start = TouchData;
        var end = new TouchDataRecord(touch.ChangedTouches[0], DateTime.Now);
        if (start != null)
        {
            var diffX = start.Point.ClientX - end.Point.ClientX;
            var diffTime = end.Time - start.Time;
            var velocity = Math.Abs(diffX / diffTime.Milliseconds);
            var distance = Math.Abs(diffX);
            var direction = distance / diffX; // result is either -1 or 1

            Log?.LogInformation("Touch ended {velocity}, {distance}, {direction}", velocity, distance, direction);

            if (distance > 20 && velocity > 0.8)
            {
                Navigate((int)direction);
            }
        }
    }

    public bool LeftArrowActive() => currentIdx > 0;

    public bool RightArrowActive() => currentIdx < images.Count - 1;

    public async Task OnResize(decimal w, decimal h)
    {
        width = w;
        height = h;
        foreach (var image in images)
        {
            image.ViewerImageLoaded = false;
            image.Active = false;
            UpdateImage();
        }
        await Task.CompletedTask;
        StateHasChanged();
    }

    public void ShowQualitySelector()
    {
        qualitySelectorShown = !qualitySelectorShown;
    }

    public void QualityChanged(string newQuality)
    {
        qualitySelected = newQuality;
        UpdateImage();
    }

    public static void ImageLoaded(GalleryImage image)
    {
        image.ViewerImageLoaded = true;
    }

    /**
     * direction (-1: left, 1: right)
     * swipe (user swiped)
     */

    public void Navigate(int direction)
    {
        if ((direction == 1 && currentIdx < images.Count - 1) || (direction == -1 && currentIdx > 0))
        {
            if (direction == -1)
            {
                images[currentIdx].Transition = "leaveToRight";
                images[currentIdx - 1].Transition = "enterFromLeft";
            }
            else
            {
                images[currentIdx].Transition = "leaveToLeft";
                images[currentIdx + 1].Transition = "enterFromRight";
            }
            currentIdx += direction;
            UpdateImage();
            StateHasChanged();
        }
    }

    public void ShowNavigationArrows()
    {
        LeftArrowVisible = true;
        RightArrowVisible = true;
    }

    public static string IsActive(GalleryImage image)
    {
        return image.Active ? "active" : "";
    }

    public void CloseViewer()
    {
        foreach (var image in images)
        {
            image.Transition = string.Empty;
            image.Active = false;
        }
        viewerClass = "viewer-fade-out";
        Timer timer = new(500);
        timer.Elapsed += (object? sender, ElapsedEventArgs e) =>
        {
            ImageService?.ShowImageViewer(false);
        };
        timer.AutoReset = false;
        timer.Start();
    }

    public void OnKeydown(KeyboardEventArgs args)
    {
        switch (args.Code)
        {
            case "ArrowLeft":
                // navigate left
                Navigate(-1);
                break;

            case "ArrowRight":
                // navigate right
                Navigate(1);
                break;

            case "Escape":
                // esc
                CloseViewer();
                break;

            case "Home":
                // pos 1
                images[currentIdx].Transition = "leaveToRight";
                currentIdx = 0;
                images[currentIdx].Transition = "enterFromLeft";
                UpdateImage();
                break;

            case "End":
                // end
                images[currentIdx].Transition = "leaveToLeft";
                currentIdx = images.Count - 1;
                images[currentIdx].Transition = "enterFromRight";
                UpdateImage();
                break;

            default:
                break;
        }
    }

    private void UpdateImage()
    {
        // wait for animation to end
        System.Timers.Timer t = new(500);
        t.Elapsed += async (object? sender, ElapsedEventArgs e) =>
        {
            UpdateQuality();
            images[currentIdx].Active = true;
            foreach (var image in images.Where((x, i) => i != currentIdx))
            {
                image.Active = false;
                transform = 0;
            }
            await InvokeAsync(() => StateHasChanged());
        };
        t.Start();
    }

    private string GetImagePath(GalleryImage image, string quality)
    {
        return $"{Config?.GetValue<string>("StorageUrl") ?? string.Empty}/images/{image.GetPropValue<string>(quality + "Path")}";
    }

    private string GetAutoCategory()
    {
        Log?.LogInformation("Width: {width}", width);
        var category = "PreviewSmall";
        if (width > images[currentIdx].PreviewSmallWidth || height > images[currentIdx].PreviewSmallHeight)
        {
            category = "PreviewMedium";
        }
        if (width > images[currentIdx].PreviewMediumWidth || height > images[currentIdx].PreviewMediumHeight)
        {
            category = "PreviewLarge";
        }
        if (width > images[currentIdx].PreviewLargeWidth || height > images[currentIdx].PreviewLargeHeight)
        {
            category = "Raw";
        }
        Log?.LogInformation("Category: {category}", category);
        return category;
    }

    private void UpdateQuality()
    {
        categorySelected = qualitySelected switch
        {
            "auto" => GetAutoCategory(),
            "low" => "PreviewSmall",
            "mid" => "PreviewMedium",
            "high" => "PreviewLarge",
            _ => "raw"
        };
    }
}