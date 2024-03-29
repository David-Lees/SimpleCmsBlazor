using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class EditPage
{
    private AdminSortSections? sorter;

    protected override async Task OnParametersSetAsync()
    {
        StateHasChanged();
        await Task.CompletedTask;
    }

    public void SectionChanged(PageSection section, int index)
    {
        if (CurrentPage != null)
        {
            CurrentPage.Sections[index] = section;
            CurrentPageChanged.InvokeAsync();
            StateHasChanged();
        }
    }

    public static IconBase GetIcon(PageSection s)
    {
        return s.Name switch
        {
            SectionType.BannerSection => BootstrapIcon.Camera,
            SectionType.TextSection => BootstrapIcon.Justify,
            SectionType.GallerySection => BootstrapIcon.Image,
            SectionType.HtmlSection => BootstrapIcon.Code,
            SectionType.ChildrenSection => BootstrapIcon.Diagram3,
            _ => BootstrapIcon.ExclamationCircle
        };
    }

    [Parameter]
    public Page? CurrentPage { get; set; }

    [Parameter]
    public EventCallback<Page> CurrentPageChanged { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }

    public PageSection? ActiveSection { get; set; }

    public async Task Change()
    {
        await CurrentPageChanged.InvokeAsync(CurrentPage);
        await OnChange.InvokeAsync();
        StateHasChanged();
    }

    public static string SectionDescription(PageSection s)
    {
        string text = s.Html ?? s.Text ?? string.Empty;
        return text.Length > 80 ? string.Concat(text.AsSpan(0, 80), "...") : text;
    }

    protected override void OnInitialized()
    {
        if (CurrentPage != null && CurrentPage.Sections.Count > 0)
        {
            ActiveSection = CurrentPage.Sections[0];
        }
    }

    protected override void OnParametersSet()
    {
        ActiveSection = null;
    }

    public async Task AddChildren()
    {
        if (CurrentPage != null)
        {
            CurrentPage.Sections.Add(new PageSection
            {
                Name = SectionType.ChildrenSection,
                BackgroundColour = "#FFFFFF",
                Colour = "#000000",
                BackgroundAlign = "center center"
            });
            await Change();
        }
    }

    public async Task AddBanner()
    {
        if (CurrentPage != null)
        {
            CurrentPage.Sections.Add(new PageSection
            {
                Name = SectionType.BannerSection,
                Image = null
            });

            await Change();
        }
    }

    public async Task AddHtml()
    {
        if (CurrentPage != null)
        {
            CurrentPage.Sections.Add(new PageSection
            {
                Name = SectionType.HtmlSection,
                Html = string.Empty
            });
            await Change();
        }
    }

    public async Task AddGallery()
    {
        if (CurrentPage != null)
        {
            CurrentPage.Sections.Add(new PageSection
            {
                Name = SectionType.GallerySection,
                GalleryName = "Gallery",
                Images = [],
                ImageMargin = 3,
                ImageSize = 7,
                RowsPerPage = 200
            });

            await Change();
        }
    }

    public async Task AddText()
    {
        if (CurrentPage != null)
        {
            CurrentPage.Sections.Add(new PageSection
            {
                Name = SectionType.TextSection,
                Text = string.Empty,
                BackgroundColour = "#FFFFFF",
                Colour = "#000000",
                Align = "left",
            });
            await Change();
        }
    }

    public async Task Remove(PageSection s)
    {
        if (CurrentPage != null && await _messageBoxService.ConfirmAsync("Remove section", "Are you sure you want to remove this section?"))
        {
            CurrentPage.Sections.Remove(s);
            await Change();
        }
    }

    public async Task SortSections()
    {
        if (sorter != null)
        {
            await sorter.OpenAsync();
        }
    }
}