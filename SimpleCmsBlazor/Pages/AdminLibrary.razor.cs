using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;
using SimpleCmsBlazor.Shared;

namespace SimpleCmsBlazor.Pages;

[Authorize]
public partial class AdminLibrary: ComponentBase
{
    [Inject]
    public IFolderService FolderService { get; set; } = default!;

    [Inject]
    public IMediaService MediaService { get; set; } = default!;

    [Inject]
    public IHxMessageBoxService MessageBoxService { get; set; } = default!;


    string ButtonText = string.Empty;
    string Title = string.Empty;
    string PromptText = string.Empty;
    FolderSelect? folderSelect;
    TextDialog? textDialog;
    MaterialFileUpload? upload;

    public async Task AddFolder()
    {
        if (textDialog != null)
        {
            ButtonText = "Add";
            Title = $"Add folder under {currentFolder?.Name ?? "root folder"}";
            PromptText = folderName;
            var result = await textDialog.ShowAsync();
            if (result.Successful)
            {
                var newFolder = new GalleryFolder
                {
                    PartitionKey = currentFolder?.RowKey ?? Guid.Empty,
                    RowKey = Guid.NewGuid(),
                    Name = result.Value
                };
                if (FolderService != null)
                {
                    await FolderService.CreateAsync(newFolder);
                    await Reload();
                }
            }
        }
    }

    public async Task RenameFolder()
    {
        if (currentFolder != null && textDialog != null)
        {
            ButtonText = "Rename";
            Title = $"Rename {currentFolder.Name}";
            PromptText = currentFolder.Name;
            var result = await textDialog.ShowAsync();
            if (result.Successful && currentFolder.Name != result.Value)
            {
                currentFolder.Name = result.Value;
                await FolderService.CreateAsync(currentFolder);
                await Reload();
            }
        }
    }

    public async Task DeleteFolder()
    {
        if (currentFolder != null && currentFolder.RowKey != Guid.Empty &&
            !folders.Any(x => x.PartitionKey == currentFolder.RowKey) &&
            !images.Any(x => x.PartitionKey == currentFolder.RowKey) &&
            await MessageBoxService.ConfirmAsync("Delete Folder", $"Are you sure you want to delete the {currentFolder.Name} folder?"))
        {
            await FolderService.DeleteAsync(currentFolder);
            await Reload();
        }
    }

    public bool CanDelete() => currentFolder != null &&
        !folders.Any(x => x.PartitionKey == currentFolder.RowKey) &&
        currentFolder.RowKey != Guid.Empty &&
        images.Count == 0;

    public bool CanRename() => currentFolder != null && currentFolder.RowKey != Guid.Empty;

    GalleryFolder? currentFolder;
    List<GalleryFolder> folders = new();
    bool showFolders = true;
    List<GalleryImage> images = new();
    readonly string folderName = "New Folder";

    protected override async Task OnInitializedAsync()
    {
        await LoadFoldersAsync();
    }

    public async Task FolderChange(GalleryFolder? folder)
    {
        currentFolder = folder;
        images = folder != null ? await FolderService.GetImagesAsync(folder) : new();
        StateHasChanged();
    }

    public async Task LoadFoldersAsync()
    {
        folders = await FolderService.GetFoldersAsync();
        await FolderChange(folders.First(x => x.RowKey == Guid.Empty));
    }

    public async Task Delete(GalleryImage image)
    {
        await MediaService.Delete(image);
        await FolderChange(currentFolder);
    }

    public async Task Reload()
    {
        var current = currentFolder?.RowKey;
        folders = await FolderService.GetFoldersAsync(true);
        var newFolder = folders.FirstOrDefault(x => x.RowKey == current);
        if (newFolder != null)
        {
            await FolderChange(newFolder);
        }
    }

    private async Task Upload()
    {
        if (upload != null)
        {
            await upload.ShowAsync();
            await FolderChange(currentFolder);
        }
    }

    public async Task Move(GalleryImage image)
    {
        if (folderSelect != null)
        {
            var result = await folderSelect.ShowAsync();
            if (result != null && result.Successful)
            {
                await MediaService.Move(image, result.Value);
            }
            StateHasChanged();
        }
    }
}
