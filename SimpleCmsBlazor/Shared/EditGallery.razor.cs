using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;

namespace SimpleCmsBlazor.Shared
{
    public partial class EditGallery
    {
        [Inject]
        public IConfiguration Config { get; set; } = default!;

        [Inject]
        public FolderService FolderService { get; set; } = default!;

        [Inject]
        public DragDropService DD { get; set; } = default!;

        [Parameter]
        public PageSection? Section { get; set; }

        [Parameter]
        public EventCallback<PageSection> SectionChanged { get; set; }

        [Parameter]
        public EventCallback OnChange { get; set; }

        private GalleryFolder? CurrentFolder;
        private List<GalleryFolder> AllFolders = [];
        private List<GalleryImage> AvailableImages = [];
        private string prefix = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            prefix = $"{Config?.GetValue<string>("StorageUrl") ?? string.Empty}/images/";
            AllFolders = await FolderService.GetFoldersAsync();
            CurrentFolder = AllFolders.FirstOrDefault();
            if (CurrentFolder != null) AvailableImages = await FolderService.GetImagesAsync(CurrentFolder);
        }

        public async Task FolderChange(GalleryFolder folder)
        {
            CurrentFolder = folder;
            AvailableImages = await FolderService.GetImagesAsync(CurrentFolder);
            Change();
        }

        public void Add(GalleryImage image)
        {
            if (Section != null && Section.Images != null) Section.Images.Add((GalleryImage)image.Clone());
            Change();
        }

        public void Remove(GalleryImage image)
        {
            if (Section != null && Section.Images != null) Section.Images.Remove(image);
            Change();
        }

        public void Up(int x)
        {
            if (x > 0)
            {
                Move(x, x - 1);
            }
        }

        public void Down(int x)
        {
            if (Section != null && Section.Images != null && x < Section.Images.Count)
            {
                Move(x, x + 1);
            }
        }

        public void Move(int from, int to)
        {
            if (Section != null && Section.Images != null)
            {
                var temp = Section.Images[from];
                Section.Images.RemoveAt(from);
                Section.Images.Insert(to, temp);
                Change();
            }
        }

        public void Change()
        {
            OnChange.InvokeAsync(null);
            StateHasChanged();
        }
    }
}