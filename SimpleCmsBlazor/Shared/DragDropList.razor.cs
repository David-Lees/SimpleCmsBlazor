using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class DragDropList<TItem> : IDisposable
{
    #region Parameters

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? Options { get; set; }

    [Parameter]
    public RenderFragment<TItem>? ItemTemplate { get; set; }

    [Parameter]
    public string ItemTemplateClass { get; set; } = "";

    [Parameter]
    public IList<TItem> Items { get; set; } = new List<TItem>();

    [Parameter]
    public EventCallback<TItem> OnItemAdded { get; set; }

    [Parameter]
    public EventCallback<TItem> OnCapacityReachedRejected { get; set; }

    [Parameter]
    public EventCallback<Tuple<int, int>> OnItemSwap { get; set; }

    [Parameter]
    public DefaultDrop DefaultDropMode { get; set; } = DefaultDrop.End;

    [Parameter]
    public RenderFragment? EmptyView { get; set; }

    [Parameter]
    public int Capacity { get; set; } = int.MaxValue;

    [Parameter]
    public bool AllowsDrag { get; set; } = true;

    [Parameter]
    public Func<TItem, TItem>? Copy { get; set; }

    [Parameter]
    public Func<TItem, bool>? Verify { get; set; }

    [Parameter]
    public EventCallback<TItem> OnRejected { get; set; }

    [Parameter]
    public string ChildName { get; set; } = string.Empty;

    #endregion

    private readonly Dictionary<int, bool> overBefore = new();
    private readonly Dictionary<int, bool> over = new();
    private readonly Dictionary<int, bool> overAfter = new();

    void DropOnList(TItem data, TItem _)
    {
        if (DefaultDropMode == DefaultDrop.Start) Items.Insert(0, data);
        if (DefaultDropMode == DefaultDrop.End) Items.Add(data);
    }

    void DropOnItem(TItem data, TItem target)
    {
        var children = target.GetPropValue<List<TItem>>(ChildName);
        if (children != null)
        {
            if (DefaultDropMode == DefaultDrop.Start) children.Insert(0, data);
            if (DefaultDropMode == DefaultDrop.End) children.Add(data);
        }
    }

    void DropBeforeItem(TItem data, TItem target)
    {
        var index = Items.IndexOf(target);
        Items.Insert(Math.Max(0, index), data);
    }

    void DropAfterItem(TItem data, TItem target)
    {
        var index = Items.IndexOf(target);
        if (index + 1 < Items.Count)
        {
            Items.Insert(Math.Max(0, index + 1), data);
        }
        else
        {
            Items.Add(data);
        }
    }

    async Task Drop(Action<TItem, TItem> DropLogic)
    {
        await Drop(DropLogic, new TItem());
    }

    async Task Drop(Action<TItem, TItem> DropLogic, TItem target)
    {
        over.Clear();
        overAfter.Clear();
        overBefore.Clear();
        StateHasChanged();

        var transfer_settings = ((IDraggable)DD).GetSettings();
        if (!transfer_settings.Me) return;

        // Get data
        TItem? data;
        try
        {
            data = (TItem?)transfer_settings.Data;
            if (data == null)
            {
                DD.Clear();
                return;
            }
        }
        catch (InvalidCastException)
        {
            DD.Clear();
            return;
        }

        // Verify we can drop
        if (Verify != null && data != null && !Verify.Invoke(data))
        {
            await OnRejected.InvokeAsync(data);
            DD.Clear();
            return;
        }

        // Abort if list is full
        var has_place = await HasPlace(data);
        if (!has_place)
        {
            DD.Clear();
            return;
        }

        // Remove old item if it comes from this or another list
        if (transfer_settings.DataSource is List<TItem> sourceList)
        {
            sourceList.Remove(data);
            transfer_settings.StateHasChanged?.Invoke();
        }

        // Do the move
        if (DropLogic != null && data != null && target != null) DropLogic(data, target);

        await OnItemAdded.InvokeAsync(data);
        DD.Clear();
        StateHasChanged();
    }

    void DragItem(TItem item)
    {
        var transfer_settings = new DataTransferSettings()
        {
            Me = true,
            HasCopy = (Copy != null),
            DataSource = this.Items,
            StateHasChanged = this.StateHasChanged,
            Data = (Copy != null) ? Copy(item) : item
        };
        ((IDraggable)DD).StartDrag(transfer_settings);
    }


    private IDisposable? _draggingSubscription = null;
    private bool _dragging = false;

    protected override void OnInitialized()
    {
        _draggingSubscription = DD.Dragging.Subscribe(x =>
        {
            _dragging = x;
            StateHasChanged();
        });
    }

    private async Task<bool> HasPlace(TItem? data)
    {
        if (Items.Count + 1 <= Capacity) return true;
        await OnCapacityReachedRejected.InvokeAsync(data);
        return false;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _draggingSubscription?.Dispose();
    }
}
