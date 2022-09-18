using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;

namespace SimpleCmsBlazor.Shared;

public partial class SelectImage: HxDialogBase<GalleryImage>
{
    [Inject]
    public IFolderService FolderService { get; set; } = default!;

    [Parameter] public GalleryImage? Selection { get; set; }
    [Parameter] public EventCallback<GalleryImage?> SelectionChanged { get; set; }

    List<GalleryFolder> folders = new();
    GalleryFolder? currentFolder;
    List<GalleryImage> images = new();

    protected override async Task OnInitializedAsync()
    {
        folders = await FolderService.GetFoldersAsync();
    }

    public async Task SelectFolder(GalleryFolder folder)
    {
        currentFolder = folder;
        images = await FolderService.GetImagesAsync(currentFolder);
    }

    public async Task Select(GalleryImage image)
    {
        await HideAsync(image);
    }
}
