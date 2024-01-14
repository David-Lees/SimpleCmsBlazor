using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class MediaTree : IDisposable
{
    private readonly List<GalleryFolder> folders = [];

    [Parameter]
    public GalleryFolder? SelectedFolder { get; set; }

    [Parameter]
    public EventCallback<GalleryFolder> NodesChanged { get; set; }

    [Parameter]
    public EventCallback<GalleryFolder> SelectedChanged { get; set; }

    [Parameter]
    public bool CanMove { get; set; } = false;

    private readonly List<Guid> expandedNodes = [];

    private readonly GalleryFolder root = new()
    {
        Name = "Library",
        Level = 0,
        PartitionKey = Guid.Empty,
        RowKey = Guid.Empty
    };

    private bool disposedValue;

    private IDisposable? _reloadSubscription = null;

    public async Task SelectNode(GalleryFolder folder)
    {
        if (expandedNodes.Contains(folder.RowKey))
        {
            expandedNodes.Remove(folder.RowKey);
        }
        else
        {
            expandedNodes.Add(folder.RowKey);
        }

        SelectedFolder = folder;
        await SelectedChanged.InvokeAsync(folder);
    }

    protected override async Task OnInitializedAsync()
    {
        _reloadSubscription = folderService.ReloadSubscription.Subscribe(async (x) =>
        {
            folders.Clear();
            folders.AddRange(await folderService.GetFoldersAsync());
            SelectedFolder ??= folders.Find(y => y.RowKey == Guid.Empty);
            expandedNodes.Add(Guid.Empty);
        });
        folders.AddRange(await folderService.GetFoldersAsync());
        SelectedFolder ??= folders.Find(y => y.RowKey == Guid.Empty);
        expandedNodes.Add(Guid.Empty);
    }

    public GalleryFolder GetRoot()
    {
        return folders.First(
          x => x.PartitionKey == Guid.Empty && x.RowKey == Guid.Empty
        );
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _reloadSubscription?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}