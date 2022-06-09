using Microsoft.JSInterop;
using System.Reactive.Subjects;

namespace SimpleCmsBlazor.Services;

public interface IBrowserResizeService
{
    IObservable<(decimal, decimal)> OnResize { get; }
    Task<decimal> GetInnerHeight();
    Task<decimal> GetInnerWidth();
}

public class BrowserResizeService : IBrowserResizeService
{
    private readonly Subject<(decimal, decimal)> onResize = new();
    public IObservable<(decimal, decimal)> OnResize => onResize;
    private readonly IJSRuntime _runtime;

    public BrowserResizeService(IJSRuntime runtime)
    {
        _runtime = runtime;
    }

    [JSInvokable]
    public void OnBrowserResize(decimal width, decimal height)
    {
        onResize.OnNext((width, height));
    }

    public async Task<decimal> GetInnerHeight()
    {
        return await _runtime.InvokeAsync<decimal>("browserResize.getInnerHeight");
    }

    public async Task<decimal> GetInnerWidth()
    {
        return await _runtime.InvokeAsync<decimal>("browserResize.getInnerWidth");
    }
}
