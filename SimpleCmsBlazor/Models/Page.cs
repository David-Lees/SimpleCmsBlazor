namespace SimpleCmsBlazor.Models;

public class Page
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public List<Section> Sections { get; set; } = new();
    public List<Page> Pages { get; set; } = new();
}
