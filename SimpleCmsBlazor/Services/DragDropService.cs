using SimpleCmsBlazor.Models;
using System.Reactive.Subjects;

namespace SimpleCmsBlazor.Services;

public class DragDropService : IDraggable
{
    private readonly Subject<bool> _dragging = new();

    public IObservable<bool> Dragging => _dragging;

    private DataTransferSettings Settings = new();

    public void SetData(object data)
    {
        Settings.Data = data;
        Settings.HasCopy = false;
        Settings.DataSource = null;
        Settings.StateHasChanged = null;
        Settings.Me = true;
        _dragging.OnNext(true);
    }
    
    public void Clear()
    {
        Settings.Clear();
        _dragging.OnNext(false);
    }

    void IDraggable.StartDrag(DataTransferSettings settings)
    {
        Settings = settings;
        _dragging.OnNext(true);
    }

    DataTransferSettings IDraggable.GetSettings() => Settings;
}
