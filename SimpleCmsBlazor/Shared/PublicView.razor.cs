using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class PublicView
{
    [Parameter]
    public string? Path { get; set; }
    private string? _path;

    private Page? Page;
    private Site Site = new();

    protected override async Task OnInitializedAsync()
    {
        //Id = NavManager.Uri
        Site = await SiteService.GetSiteAsync();
        _path = Path;
        Page = FindPage(Path);
    }

    protected override void OnParametersSet()
    {
        if (Path != _path)
        {
            _path = Path;
            Page = FindPage(Path);
        }
    }

    private Page FindPage(string? url)
    {
        if (string.IsNullOrEmpty(url) || url == "/")
            return Site.Pages[0];

        var urls = url.Split('/');
        var page = Site as IPageList;
        Array.ForEach(urls, x => page = FindChildPage(x, page));
        if (page == Site) page = Site.Pages[0];
        return (Page)page ?? Site.Pages[0];
    }

    private static Page? FindChildPage(string url, IPageList page)
    {
        if (page != null && page.Pages != null)
            return page.Pages.FirstOrDefault(x => x.Url?.ToLowerInvariant() == url.ToLowerInvariant());

        return null;
    }

    public static string GetYear() => DateTime.Now.Year.ToString();
}
