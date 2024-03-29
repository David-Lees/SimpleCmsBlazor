using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class EditHtml
{
    private string data = string.Empty;

    [Parameter]
    public PageSection? Section { get; set; }

    [Parameter]
    public EventCallback<PageSection> SectionChanged { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }

    private async Task HandleOnChange(ChangeEventArgs args)
    {
        data = args.Value?.ToString() ?? string.Empty;
        if (Section != null)
        {
            Section.Html = data;
            await SectionChanged.InvokeAsync(Section);
            await OnChange.InvokeAsync(null);
        }
    }

    protected override void OnParametersSet()
    {
        if (Section != null)
        {
            data = Section.Html ?? string.Empty;
        }
        base.OnParametersSet();
    }
}