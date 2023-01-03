using System.Text.Json.Serialization;

namespace SimpleCmsBlazor.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SectionType
{
    TextSection,
    GallerySection,
    BannerSection,
    HtmlSection,
    ChildrenSection
}
