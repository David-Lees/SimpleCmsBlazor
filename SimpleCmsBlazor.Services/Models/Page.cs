namespace SimpleCmsBlazor.Models;

public interface IPageList
{
    List<Page> Pages { get; }
    string? Id { get; }
    string? Name { get; }
}

public class Page : IPageList
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public List<PageSection> Sections { get; set; } = new();
    public List<Page> Pages { get; set; } = new();
}
