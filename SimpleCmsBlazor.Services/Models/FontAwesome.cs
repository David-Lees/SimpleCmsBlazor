using Havit.Blazor.Components.Web;
using SimpleCmsBlazor.Services.Shared;

namespace SimpleCmsBlazor.Models;

public class FontAwesome : IconBase
{
    public override Type RendererComponentType => typeof(FontAwesomeIcon);

    public string Icon { get; set; } = string.Empty;

    public static FontAwesome Folder { get; set; } = new() { Icon = "fa-solid fa-folder" };
    public static FontAwesome FolderOpen { get; set; } = new() { Icon = "fa-solid fa-folder-open" };
}
