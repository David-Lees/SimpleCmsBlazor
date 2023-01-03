using SimpleCmsBlazor.Models;
using System.Reactive.Subjects;

namespace SimpleCmsBlazor.Services;

public interface IImageService
{
    IObservable<int> ImagesSelectedIndexUpdated { get; }
    IObservable<List<GalleryImage>> ImagesUpdated { get; }
    IObservable<bool> ShowImageViewerChanged { get; }

    void ShowImageViewer(bool showImageViewer);
    void UpdateImages(List<GalleryImage> images);
    void UpdateSelectedImageIndex(int newIndex);
}

public class ImageService : IImageService
{
    private readonly Subject<List<GalleryImage>> imagesUpdatedSource = new();
    private readonly Subject<int> imagesSelectedIndexUpdatedSource = new();
    private readonly Subject<bool> showImageViewerSource = new();

    public IObservable<List<GalleryImage>> ImagesUpdated => imagesUpdatedSource;
    public IObservable<int> ImagesSelectedIndexUpdated => imagesSelectedIndexUpdatedSource;
    public IObservable<bool> ShowImageViewerChanged => showImageViewerSource;

    public void UpdateImages(List<GalleryImage> images) => imagesUpdatedSource.OnNext(images);
    public void UpdateSelectedImageIndex(int newIndex) => imagesSelectedIndexUpdatedSource.OnNext(newIndex);
    public void ShowImageViewer(bool showImageViewer) => showImageViewerSource.OnNext(showImageViewer);
}
