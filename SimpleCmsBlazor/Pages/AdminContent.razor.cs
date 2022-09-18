using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Services;

namespace SimpleCmsBlazor.Pages;

[Authorize]
public partial class AdminContent : ComponentBase
{
    [Inject] public ISiteService SiteService { get; set; } = default!;
    [Inject] public IMediaService MediaService { get; set; } = default!;
    [Inject] public IHxMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] public ILogger<AdminContent> Log { get; set; } = default!;

    public bool Dragging { get; set; } = false;

    private bool showPages = true;
    public Page? ActivePage { get; set; }
    public Site? Site { get; set; }
    private readonly List<string> expandedNodes = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    public void AddPage()
    {
        if (Site != null)
        {
            Site.Pages.Add(NewPage());
        }
    }

    public void AddChild()
    {
        ActivePage?.Pages.Add(NewPage());
    }

    public bool CanDelete() => ActivePage != null;

    private static Page NewPage()
    {
        return new Page
        {
            Id = Guid.NewGuid().ToString(),
            Name = "New page",
            Url = "new-page",
            Sections = new(),
            Pages = new(),
        };
    }

    public void Remove(Page p)
    {
        if (Site != null && Site.Pages.Contains(p))
        {
            Site.Pages.Remove(p);
        }
    }

    public async Task SaveAsync()
    {
        if (Site != null)
        {
            await SiteService.SaveSiteAsync(Site);
        }
    }

    private async Task LoadAsync()
    {
        if (SiteService != null)
        {
            Site = await SiteService.GetSiteAsync();
            if (string.IsNullOrEmpty(Site.Id)) Site.Id = Guid.NewGuid().ToString();            
            ActivePage = Site.Pages.FirstOrDefault();
            expandedNodes.Add(Site.Id);
        }
    }

    public bool IsExpanded(string id)
    {
        return expandedNodes.Contains(id);
    }

    public void NodeClick(Page? node)
    {
        if (node != null && !string.IsNullOrEmpty(node.Id))
        {
            ActivePage = node;
            StateHasChanged();
        }
    }

    public IPageList? GetNode(string id, IPageList? nodesToSearch = null)
    {
        if (id == "main") return Site;
        nodesToSearch ??= Site;
        if (nodesToSearch != null)
        {
            foreach (var node in nodesToSearch.Pages)
            {
                if (node.Id == id) return node;
                var ret = GetNode(id, node);
                if (ret != null) return ret;
            }
        }
        return null;
    }

    public bool AcceptDrop(Page droppedItem)
    {
        if (Site == null) return false;
        if (ActivePage == null)
        {
            return true;
        }
        var allowed = !ContainsPage(Site, droppedItem);
        return allowed;
    }

    private bool ContainsPage(IPageList item, IPageList parent)
    {
        if (parent.Pages.Contains(item)) return true;
        return parent.Pages.Any(p => ContainsPage(item, p));
    }

    public IPageList? GetParentNode(string id, IPageList? nodesToSearch = null)
    {
        if (id == "main") return Site;
        nodesToSearch ??= Site;
        if (nodesToSearch != null)
        {
            foreach (var node in nodesToSearch.Pages ?? new())
            {
                if (node.Id == id) return nodesToSearch;
                var result = GetParentNode(id, node);
                if (result != null) return result;
            }
        }
        return null;
    }

    public async Task DeletePage()
    {
        if (ActivePage != null && await MessageBoxService.ConfirmAsync("Confirm Delete", "Are you sure you want to delete this page?"))
        {
            var sourceFolder = GetParentNode(ActivePage.Id ?? string.Empty);
            if (sourceFolder != null)
            {
                var i = sourceFolder.Pages.FirstOrDefault(c => c.Id == ActivePage.Id);
                if (i != null)
                {
                    sourceFolder.Pages.Remove(i);
                }
            }
        }
    }
}
