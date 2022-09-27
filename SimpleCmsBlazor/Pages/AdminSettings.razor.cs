using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;

namespace SimpleCmsBlazor.Pages;

[Authorize]
public partial  class AdminSettings : ComponentBase
{
    [Inject]
    public ISiteService SiteService { get; set; } = default!;

    Site site = new();
    bool loaded = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    public async Task SaveAsync()
    {
        await SiteService.SaveSiteAsync(site);
    }

    public async Task LoadAsync()
    {
        site = await SiteService.GetSiteAsync();
        loaded = true;
        StateHasChanged();
    }
}
