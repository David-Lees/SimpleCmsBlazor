using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;
using SimpleCmsBlazor.Shared;

namespace SimpleCmsBlazor.Pages;

[Authorize]
public partial class AdminLibrary : ComponentBase
{
    [Inject]
    public ILogger<AdminLibrary> Logger { get; set; } = default!;

    [Inject]
    public FolderService FolderService { get; set; } = default!;

    [Inject]
    public IMediaService MediaService { get; set; } = default!;

    [Inject]
    public IHxMessageBoxService MessageBoxService { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    private string ButtonText = string.Empty;
    private string Title = string.Empty;
    private string PromptText = string.Empty;
    private FolderSelect? folderSelect;
    private TextDialog? textDialog;
    private MaterialFileUpload? upload;

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
            !folders.Exists(x => x.PartitionKey == currentFolder.RowKey) &&
            !images.Exists(x => x.PartitionKey == currentFolder.RowKey) &&
            await MessageBoxService.ConfirmAsync("Delete Folder", $"Are you sure you want to delete the {currentFolder.Name} folder?"))
        {
            await FolderService.DeleteAsync(currentFolder);
            await Reload();
        }
    }

    public bool CanDelete() => currentFolder != null &&
        !folders.Exists(x => x.PartitionKey == currentFolder.RowKey) &&
        currentFolder.RowKey != Guid.Empty &&
        images.Count == 0;

    public bool CanRename() => currentFolder != null && currentFolder.RowKey != Guid.Empty;

    private GalleryFolder? currentFolder;
    private List<GalleryFolder> folders = [];
    private bool showFolders = true;
    private List<GalleryImage> images = [];
    private readonly string folderName = "New Folder";

    protected override async Task OnInitializedAsync()
    {
        await FolderChange(FolderService.CurrentFolder);
        await LoadFoldersAsync();
    }

    public async Task FolderChange(GalleryFolder? folder)
    {
        currentFolder = folder;
        FolderService.CurrentFolder = folder;
        images = folder != null ? await FolderService.GetImagesAsync(folder) : [];
        StateHasChanged();
    }

    public async Task LoadFoldersAsync()
    {
        folders = await FolderService.GetFoldersAsync();
        if (currentFolder is null) await FolderChange(folders.First(x => x.RowKey == Guid.Empty));
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
        var newFolder = folders.Find(x => x.RowKey == current);
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
        Logger.LogInformation("Move called for image {Image}", image.Description);
        if (folderSelect != null)
        {
            var result = await folderSelect.ShowAsync();
            if (result != null && result.Successful)
            {
                await MediaService.Move(image, result.Value);
                await Reload();
            }
            StateHasChanged();
        }
    }

    public void Edit(GalleryImage image)
    {
        Navigation.NavigateTo($"/admin/library/image/{image.PartitionKey}/{image.RowKey}");
    }
}