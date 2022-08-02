using System.Text.Json.Serialization;

namespace SimpleCmsBlazor.Models
{
    public class GalleryImage
    {
        public Guid? PartitionKey { get; set; }
        public Guid? RowKey { get; set; }

        public string? PreviewSmallPath { get; set; }
        public decimal? PreviewSmallWidth { get; set; }
        public decimal? PreviewSmallHeight { get; set; }

        public string? PreviewMediumPath { get; set; }
        public decimal? PreviewMediumWidth { get; set; }
        public decimal? PreviewMediumHeight { get; set; }

        public string? PreviewLargePath { get; set; }
        public decimal? PreviewLargeWidth { get; set; }
        public decimal? PreviewLargeHeight { get; set; }

        public string? RawPath { get; set; }
        public decimal? RawWidth { get; set; }
        public decimal? RawHeight { get; set; }

        public string? DominantColour { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public bool GalleryImageLoaded { get; set; } = false;

        [JsonIgnore]
        public bool ViewerImageLoaded { get; set; } = false;

        [JsonIgnore]
        public string SrcAfterFocus { get; set; } = string.Empty;

        [JsonIgnore]
        public decimal Width { get; set; } = 0;

        [JsonIgnore]
        public decimal Height { get; set; } = 0;

        [JsonIgnore]
        public bool Active { get; set; } = false;

        [JsonIgnore]
        public string Transition { get; set; } = string.Empty;
    }
}
