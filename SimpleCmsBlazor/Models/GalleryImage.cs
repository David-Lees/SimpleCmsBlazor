namespace SimpleCmsBlazor.Models
{
    public class GalleryImage
    {
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }

        public string? PreviewSmallPath { get; set; }
        public int? PreviewSmallWidth { get; set; }
        public int? PreviewSmallHeight { get; set; }

        public string? PreviewMediumPath { get; set; }
        public int? PreviewMediumWidth { get; set; }
        public int? PreviewMediumHeight { get; set; }

        public string? PreviewLargePath { get; set; }
        public int? PreviewLargeWidth { get; set; }
        public int? PreviewLargeHeight { get; set; }

        public string? RawPath { get; set; }
        public int? RawWidth { get; set; }
        public int? RawHeight { get; set; }

        public string? DominantColour { get; set; }
        public string? Description { get; set; }
    }
}
