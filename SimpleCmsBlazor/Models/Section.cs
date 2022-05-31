namespace SimpleCmsBlazor.Models;

public class Section
{
    public string? Name { get; set; }
}

public class HtmlSection : Section
{
    public string? Html { get; set; }
}

public class GallerySection : Section
{
    public List<GalleryImage> Images { get; set; } = new();
    public int? ImageMargin { get; set; }
    public int? ImageSize { get; set; }
    public string? GalleryName { get; set; }
    public int? RowsPerPage { get; set; }
}

public class BannerSection : Section
{
    public GalleryImage? Image { get; set; }
}

public class ChildrenSection : BannerSection
{
    public string? BackgroundColour { get; set; }
    public string? BackgroundAlign { get; set; }
    public string? Colour { get; set; }
}

public class TextSection: BannerSection
{
    public string? Text { get; set; } // markdown
    public string? BackgroundColour { get; set; }
    public string? BackgroundAlign { get; set; }
    public string? Colour { get; set; }
    public string? Align { get; set; }
}