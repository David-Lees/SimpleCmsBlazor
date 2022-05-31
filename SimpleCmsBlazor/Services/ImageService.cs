using System.Reactive.Subjects;

namespace SimpleCmsBlazor.Services;

public interface IImageService
{
    IObservable<int> ImagesSelectedIndexUpdated { get; }
    IObservable<object[]> ImagesUpdated { get; }
    IObservable<bool> ShowImageViewerChanged { get; }

    void ShowImageViewer(bool showImageViewer);
    void UpdateImages(object[] images);
    void UpdateSelectedImageIndex(int newIndex);
}

public class ImageService : IImageService
{
    private readonly Subject<object[]> imagesUpdatedSource = new();
    private readonly Subject<int> imagesSelectedIndexUpdatedSource = new();
    private readonly Subject<bool> showImageViewerSource = new();

    public IObservable<object[]> ImagesUpdated => imagesUpdatedSource;
    public IObservable<int> ImagesSelectedIndexUpdated => imagesSelectedIndexUpdatedSource;
    public IObservable<bool> ShowImageViewerChanged => showImageViewerSource;

    public void UpdateImages(object[] images) => imagesUpdatedSource.OnNext(images);
    public void UpdateSelectedImageIndex(int newIndex) => imagesSelectedIndexUpdatedSource.OnNext(newIndex);
    public void ShowImageViewer(bool showImageViewer) => showImageViewerSource.OnNext(showImageViewer);
}
