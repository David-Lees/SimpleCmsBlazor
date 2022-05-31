namespace SimpleCmsBlazor.Models;

public class PageSection
{
    public SectionType Name { get; set; }
    public string? Html { get; set; } // Raw Html
    public List<GalleryImage>? Images { get; set; }
    public int? ImageMargin { get; set; }
    public int? ImageSize { get; set; }
    public string? GalleryName { get; set; }
    public int? RowsPerPage { get; set; }
    public GalleryImage? Image { get; set; }
    public string? BackgroundColour { get; set; }
    public string? BackgroundAlign { get; set; }
    public string? Colour { get; set; }
    public string? Align { get; set; }
    public string? Text { get; set; } // markdown    
}