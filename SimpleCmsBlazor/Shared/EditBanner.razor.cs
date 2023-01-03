using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class EditBanner
{
    [Parameter] public PageSection? Section { get; set; }
    [Parameter] public EventCallback<PageSection> SectionChanged { get; set; }
    [Parameter] public EventCallback OnChange { get; set; }

    private PageSection? pageSection;
    SelectImage? selectImage;

    [Inject]
    public IConfiguration? Config { get; set; }

    string prefix = string.Empty;

    protected override void OnInitialized()
    {
        base.OnInitialized();
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
