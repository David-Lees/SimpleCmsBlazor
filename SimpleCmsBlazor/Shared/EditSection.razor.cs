using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class EditSection
{
    private PageSection? _section;
    ObjectsComparer.Comparer<PageSection?> _comparer = new();

    [Parameter]
    public PageSection? Section
    {
        get => _section;
        set
        {
            if (_comparer.Compare(_section, value)) return;
            _section = value;
            SectionChanged.InvokeAsync(value);
        }
    }

    [Parameter]
    public EventCallback<PageSection> SectionChanged { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }
}
