using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;

namespace SimpleCmsBlazor.Shared;

public partial class Admin
{
    private readonly ISiteService SiteService;
    private Site Site = new();

    public Admin(ISiteService siteService)
    {
        SiteService = siteService;
    }

    protected override async Task OnInitializedAsync()
    {
        Site = await SiteService.GetSiteAsync();
    }

    public static string GetYear() => DateTime.Now.Year.ToString();
}
