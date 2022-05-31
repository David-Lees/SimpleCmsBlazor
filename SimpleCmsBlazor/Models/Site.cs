namespace SimpleCmsBlazor.Models;

public class Site
{
    public string? Id { get; set; }
    public List<Page> Pages { get; set; } = new();
    public string? Name { get; set; }
    public string? HeaderBackground { get; set; }
    public string? HeaderColour { get; set; }
    public bool HasLogo { get; set; } = false;
    public bool? IsExpanded { get; set; }
}
