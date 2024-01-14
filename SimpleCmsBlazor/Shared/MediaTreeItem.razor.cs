using Microsoft.AspNetCore.Components;
using Radzen;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;

namespace SimpleCmsBlazor.Shared;

public partial class MediaTreeItem
{
    [Inject]
    public ILogger<MediaTreeItem> Logger { get; set; } = default!;

    [Inject]
    public FolderService FolderService { get; set; } = default!;

    [Parameter]
    public List<GalleryFolder> Folders { get; set; } = [];

    [Parameter]
    public GalleryFolder? Folder { get; set; }

    [Parameter]
    public GalleryFolder? ActiveFolder { get; set; }

    [Parameter]
    public EventCallback<GalleryFolder> NodeClick { get; set; }

    [Parameter]
    public List<Guid> ExpandedNodes { get; set; } = [];

    [Parameter]
    public bool CanMove { get; set; } = false;

    public bool IsExpanded(Guid id) => ExpandedNodes.Contains(id);

    public bool HasChildren(Guid parentId)
    {
        return Folders.Exists(x => x.PartitionKey == parentId && x.RowKey != Guid.Empty);
    }

    public List<GalleryFolder> GetChildren(Guid parentId)
    {
        return Folders.Where(x => x.PartitionKey == parentId && x.RowKey != Guid.Empty).ToList();
    }

    public async Task OnNodeClick()
    {
        await NodeClick.InvokeAsync(Folder);
    }

    public bool AcceptDrop(GalleryFolder drag, GalleryFolder target)
    {
        Logger.LogInformation("Checking if can accept drop on {Target}", target.Name);
        var result = CanMove && drag != Folder && !IsChildOf(drag, target);
        Logger.LogInformation("Accept drop called and outputting {Result}, {CanMove}", result, CanMove);
        return result;
    }

    private bool IsChildOf(GalleryFolder item, GalleryFolder target)
    {
        Logger.LogInformation("Checking if {Item} is child of {Target}", item.Name, target.Name);
        var isChild = IsChildOfInner(target, item);
        Logger.LogInformation("IsChildOf is {Result}", isChild);
        return isChild;
    }

    private bool IsChildOfInner(GalleryFolder target, GalleryFolder item)
    {
        var isChild = false;
        foreach (var child in GetChildren(target.RowKey))
        {
            isChild = isChild || child == item || IsChildOfInner(child, item);
        }
        return isChild;
    }

    public async Task DropItem(Tuple<GalleryFolder, GalleryFolder> item)
    {
        Logger.LogInformation("Dropped folder {Source} onto {Destination}, move initialised", item.Item1.Name, item.Item2.Name);
        if (CanMove)
        {
            await FolderService.MoveAsync(item.Item1, item.Item2.RowKey);
            StateHasChanged();
        }
    }
}