using Markdig;
using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class EditText : ComponentBase
{
    private string text = string.Empty;
    private string back = string.Empty;
    private string data = string.Empty;

    [Parameter]
    public PageSection? Section { get; set; }

    [Parameter]
    public EventCallback<PageSection> SectionChanged { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }

    private string output = string.Empty;
    private readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    private sealed class Alignment(string value, string description)
    {
        public string Value { get; set; } = value;
        public string Description { get; set; } = description;
    }

    private readonly List<Alignment> Alignments =
    [
        new Alignment("left top", "Top left"),
        new Alignment("left center", "Centre Left"),
        new Alignment("left bottom", "Bottom Left"),
        new Alignment("right top", "Top Right"),
        new Alignment("right center", "Centre Right"),
        new Alignment("right bottom", "Bottom Right"),
        new Alignment("center top", "Top Centre"),
        new Alignment("center center", "Centered"),
        new Alignment("center bottom", "Center Bottom"),
    ];

    private readonly List<Alignment> TextAlignments =
    [
        new Alignment("left", "Left"),
        new Alignment("center", "Centred"),
        new Alignment("right", "Right")
    ];

    protected override void OnParametersSet()
    {
        text = Section?.Colour ?? string.Empty;
        back = Section?.BackgroundColour ?? string.Empty;
        if (Section != null)
        {
            data = Section.Text ?? string.Empty;
        }
        UpdateOutput();
    }

    private void UpdateOutput()
    {
        output = Markdown.ToHtml(Section?.Text ?? string.Empty, pipeline);
    }

    public void Change()
    {
        text = Section?.Colour ?? string.Empty;
        back = Section?.BackgroundColour ?? string.Empty;
        OnChange.InvokeAsync(null);
        StateHasChanged();
    }

    private async Task HandleOnChange(ChangeEventArgs args)
    {
        data = args.Value?.ToString() ?? string.Empty;
        if (Section != null)
        {
            Section.Text = data;
            await SectionChanged.InvokeAsync(Section);
            UpdateOutput();
            Change();
        }
    }
}