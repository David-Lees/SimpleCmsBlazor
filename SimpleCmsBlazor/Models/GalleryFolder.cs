namespace SimpleCmsBlazor.Models;

public class GalleryFolder
{
    public Guid PartitionKey { get; set; } = Guid.Empty;
    public Guid RowKey { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public int? Level { get; set; }
}
