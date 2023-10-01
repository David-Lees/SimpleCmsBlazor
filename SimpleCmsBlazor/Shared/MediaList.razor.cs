using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared
{
    public partial class MediaList
    {
        [Parameter]
        public List<GalleryImage> Images { get; set; } = new();

        [Parameter]
        public EventCallback<List<GalleryImage>> ImagesChanged { get; set; }

        private List<GalleryImage> images = new();

        [Parameter]
        public bool CanSelect { get; set; } = false;

        [Parameter]
        public bool CanDelete { get; set; } = false;

        [Parameter]
        public bool CanMove { get; set; } = false;

        [Parameter]
        public bool CanSort { get; set; } = false;

        [Parameter]
        public bool CanEdit { get; set; } = false;

        [Parameter]
        public EventCallback<GalleryImage> Deleted { get; set; }

        [Parameter]
        public EventCallback<GalleryImage> Selected { get; set; }

        [Parameter]
        public EventCallback<GalleryImage> Moved { get; set; }

        [Parameter]
        public EventCallback<GalleryImage> Edit { get; set; }

        public string Prefix => $"{Config?.GetValue<string>("StorageUrl") ?? string.Empty}/images/";

        protected override void OnInitialized()
        {
            images = Images;
        }

        public async Task Select(GalleryImage image)
        {
            await Selected.InvokeAsync(image);
        }

        public async Task Swap(Tuple<int, int> data)
        {
            if (CanSort)
            {
                await ImagesChanged.InvokeAsync(images);
            }
        }

        public async Task Drop(GalleryImage image)
        {
            if (CanSort)
            {
                await ImagesChanged.InvokeAsync(images);
            }
        }

        public async Task Remove(GalleryImage image)
        {
            if (_messageBoxService != null && await _messageBoxService.ConfirmAsync("Confirm Deletion", $"Are you sure you want to remove {image.Description}?"))
            {
                await Delete(image);
            }
        }

        public async Task Move(GalleryImage image)
        {
            await Moved.InvokeAsync(image);
        }

        public async Task Delete(GalleryImage image)
        {
            await Deleted.InvokeAsync(image);
        }

        public async Task EditImage(GalleryImage image)
        {
            await Edit.InvokeAsync(image);
        }
    }
}