namespace SimpleCmsBlazor.Models;

public class GalleryFolder
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? Level { get; set; }
}
