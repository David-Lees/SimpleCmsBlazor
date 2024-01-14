using Microsoft.AspNetCore.Components;
using SimpleCmsBlazor.Models;

namespace SimpleCmsBlazor.Shared;

public partial class DragDropList<TItem> : IDisposable
{
    [Inject]
    public ILogger<DragDropList<TItem>> Logger { get; set; } = default!;

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
    public EventCallback<Tuple<TItem, TItem>> OnItemDroppedOnTarget { get; set; }

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
    public Func<TItem, TItem, bool>? Verify { get; set; }

    [Parameter]
    public EventCallback<TItem> OnRejected { get; set; }

    [Parameter]
    public string ChildName { get; set; } = string.Empty;

    #endregion Parameters

    private readonly Dictionary<int, bool> overBefore = [];
    private readonly Dictionary<int, bool> over = [];
    private readonly Dictionary<int, bool> overAfter = [];

    private void DropOnList(TItem data, TItem _)
    {
        Logger.LogInformation("Item dropped on list with mode of {Mode}", DefaultDropMode);
        if (DefaultDropMode == DefaultDrop.Start)
            Items.Insert(0, data);
        if (DefaultDropMode == DefaultDrop.End)
            Items.Add(data);
    }

    private void DropOnItem(TItem data, TItem target)
    {
        if (!string.IsNullOrEmpty(ChildName))
        {
            Logger.LogInformation("Item dropped on item with mode of {Mode}", DefaultDropMode);
            var children = target?.GetPropValue<List<TItem>>(ChildName);
            if (children != null)
            {
                if (DefaultDropMode == DefaultDrop.Start)
                    children.Insert(0, data);
                if (DefaultDropMode == DefaultDrop.End)
                    children.Add(data);
            }
        }
    }

    private void DropBeforeItem(TItem data, TItem target)
    {
        Logger.LogInformation("Item dropped before item");
        var index = Items.IndexOf(target);
        Items.Insert(Math.Max(0, index), data);
    }

    private void DropAfterItem(TItem data, TItem target)
    {
        Logger.LogInformation("Item dropped after item");
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

    private async Task Drop(Action<TItem, TItem> DropLogic)
    {
        Logger.LogInformation("Item dropped - invoking drop logic");
        await Drop(DropLogic, new TItem());
    }

    private async Task Drop(Action<TItem, TItem> DropLogic, TItem target)
    {
        over.Clear();
        overAfter.Clear();
        overBefore.Clear();
        StateHasChanged();
        var transfer_settings = ((IDraggable)DD).GetSettings();
        if (!transfer_settings.Me)
            return;
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
        if (Verify != null && !Verify.Invoke(data, target))
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
        try
        {
            if (transfer_settings.DataSource is List<TItem> sourceList)
            {
                sourceList.Remove(data);
                transfer_settings.StateHasChanged?.Invoke();
            }
        }
        catch (Exception)
        {
            // Ignore errors
        }

        // Do the move
        if (DropLogic != null && target != null)
            DropLogic(data, target);
        await OnItemAdded.InvokeAsync(data);
        await OnItemDroppedOnTarget.InvokeAsync(new(data, target));
        DD.Clear();
        StateHasChanged();
    }

    private void DragItem(TItem item)
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

    protected override void OnInitialized()
    {
        _draggingSubscription = DD.Dragging.Subscribe(x =>
        {
            StateHasChanged();
        });
    }

    private async Task<bool> HasPlace(TItem? data)
    {
        if (Items.Count + 1 <= Capacity)
            return true;
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
        // Cleanup
        _draggingSubscription?.Dispose();
    }
}