using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class EditChildren
{
    private string text = string.Empty;
    private string back = string.Empty;

    private sealed class Alignment
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public Alignment(string value, string description)
        {
            Value = value;
            Description = description;
        }
    }

    private readonly List<Alignment> Alignments = new()
    {
        new Alignment("left top","Top left"),
        new Alignment("left center","Centre Left"),
        new Alignment("left bottom","Bottom Left"),
        new Alignment("right top","Top Right"),
        new Alignment("right center","Centre Right"),
        new Alignment("right bottom","Bottom Right"),
        new Alignment("center top","Top Centre"),
        new Alignment("center center","Centered"),
        new Alignment("center bottom","Center Bottom"),
    };

    [Parameter]
    public PageSection? Section { get; set; }

    [Parameter]
    public EventCallback<PageSection> SectionChanged { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }

    public void UpdateImage(PageSection s)
    {
        if (Section != null)
        {
            Section.Image = s.Image;
            SectionChanged.InvokeAsync(Section);
            Change();
        }
    }

    public void Change()
    {
        text = Section?.Colour ?? string.Empty;
        back = Section?.BackgroundColour ?? string.Empty;
        OnChange.InvokeAsync(null);
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        text = Section?.Colour ?? string.Empty;
        back = Section?.BackgroundColour ?? string.Empty;
    }
}
