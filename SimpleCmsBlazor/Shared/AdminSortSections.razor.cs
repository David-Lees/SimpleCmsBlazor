using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class AdminSortSections
{
    [Parameter]
    public Page? CurrentPage { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }

    public static BootstrapIcon GetIcon(PageSection section)
    {
        return section.Name switch
        {
            SectionType.BannerSection => BootstrapIcon.Camera,
            SectionType.TextSection => BootstrapIcon.Justify,
            SectionType.GallerySection => BootstrapIcon.ViewList,
            SectionType.HtmlSection => BootstrapIcon.Code,
            SectionType.ChildrenSection => BootstrapIcon.List,
            _ => BootstrapIcon.Exclamation
        };
    }

    private HxOffcanvas? canvas;

    public async Task OpenAsync()
    {
        if (canvas != null) await canvas.ShowAsync();
    }

    public void Change()
    {
        OnChange.InvokeAsync();
    }

    public static string SectionDescription(PageSection s)
    {
        string text = s.Html ?? s.Text ?? string.Empty;
        return text.Length > 80 ? string.Concat(text.AsSpan(0, 80), "...") : text;
    }
}
