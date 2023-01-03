namespace SimpleCmsBlazor.Models;

public class DataTransferSettings
{
    //Indicates if the source has a copy function.
    public bool HasCopy { get; set; } = false;

    //A List of Items(Only if the data source is a DragDrop List)
    public object? DataSource { get; set; }

    //the state has changed event of the data source component.
    public Action? StateHasChanged { get; set; }

    //The Dragged Data
    public object? Data { get; set; }

    //this boolean is Set to true when the drag event occurs using this service, since there can be other drag service
    public bool Me { get; set; } = false;

    public void Clear()
    {
        HasCopy = false;
        DataSource = null;
        StateHasChanged = null;
        Data = null;
        Me = false;
    }
}
