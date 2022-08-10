namespace SimpleCmsBlazor.Models;

public interface IDraggable
{
    public void StartDrag(DataTransferSettings settings);
    public DataTransferSettings GetSettings();
}
