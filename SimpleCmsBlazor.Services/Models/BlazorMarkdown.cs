using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace SimpleCmsBlazor.Models;

public class BlazorMarkdown : ComponentBase
{
    [Parameter]
    public string Text { get; set; } = string.Empty;

    private MarkupString _markupString = new();

    public virtual MarkdownPipeline Pipeline => new MarkdownPipelineBuilder()
        .UseEmojiAndSmiley()
        .UseAdvancedExtensions()
        .UseBootstrap()
        .Build();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.AddContent(0, _markupString);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _markupString = new MarkupString(Markdown.ToHtml(Text, Pipeline));
        StateHasChanged();
    }
}
