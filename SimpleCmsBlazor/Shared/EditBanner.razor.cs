using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class EditBanner
{
    [Parameter] public PageSection? Section { get; set; }
    [Parameter] public EventCallback<PageSection> SectionChanged { get; set; }
    [Parameter] public EventCallback OnChange { get; set; }

    private PageSection? pageSection;
    private SelectImage? selectImage;

    [Inject]
    public IConfiguration? Config { get; set; }

    private string prefix = string.Empty;

    protected override void OnParametersSet()
    {
        pageSection = Section;
        prefix = $"{Config?.GetValue<string>("StorageUrl") ?? string.Empty}/images/";
    }

    public async Task Select()
    {
        if (selectImage != null)
        {
            var result = await selectImage.ShowAsync();
            if (pageSection != null && result.Successful)
            {
                pageSection.Image = result.Value;
                await SectionChanged.InvokeAsync(pageSection);
                await OnChange.InvokeAsync(null);
            }
        }
    }
}