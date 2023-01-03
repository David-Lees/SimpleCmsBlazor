using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class EditHtml
{
    private readonly IJSRuntime _jsRuntime;
    private ElementReference myInput;

    public EditHtml(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    [Parameter]
    public PageSection? Section { get; set; }

    [Parameter]
    public EventCallback<PageSection> SectionChanged { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _jsRuntime.InvokeVoidAsync("codemirror.create", myInput, Section?.Html ?? "Error", "htmlmixed", DotNetObjectReference.Create(this));
        }
    }

    [JSInvokable("UpdateField")]
    public Task UpdateField(string codeValue)
    {
        if (Section != null)
        {
            Section.Html = codeValue;
            SectionChanged.InvokeAsync(Section);
            OnChange.InvokeAsync(null);

        }
        return Task.CompletedTask;
    }
}
