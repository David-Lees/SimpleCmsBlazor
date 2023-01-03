using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;
using SimpleCmsBlazor.Pages;

namespace SimpleCmsBlazor.Shared;

public partial class AdminContentItem
{
    [CascadingParameter]
    public AdminContent? Parent { get; set; }

    [Parameter]
    public IPageList? Node { get; set; }

    [Parameter]
    public Page? ActivePage { get; set; }

    [Parameter]
    public EventCallback<Page> NodeClick { get; set; }

    [Parameter]
    public List<string> ExpandedNodes { get; set; } = new();

    public bool IsExpanded(string id) => ExpandedNodes.Contains(id);

    public async Task OnNodeClick()
    {
        await NodeClick.InvokeAsync(Node as Page);
        StateHasChanged();
    }

    public bool AcceptDrop(Page droppedItem)
    {
        if (Node == null)
        {
            return false;
        }
        var allowed = !ContainsPage(Node, droppedItem);
        return allowed;
    }

    private bool ContainsPage(IPageList item, IPageList parent)
    {
        if (parent.Pages.Contains(item)) return true;
        return parent.Pages.Any(p => ContainsPage(item, p));
    }

    public void StartDrag()
    {
        if (Parent != null) Parent.Dragging = true;
        StateHasChanged();
    }

    public void EndDrag()
    {
        if (Parent != null) Parent.Dragging = false;
        StateHasChanged();
    }
}
