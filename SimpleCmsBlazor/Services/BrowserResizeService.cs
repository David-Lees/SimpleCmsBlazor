using Microsoft.JSInterop;

namespace SimpleCmsBlazor.Services;

public interface IBrowserResizeService
{
    event Func<Task>? OnResize;

    Task<int> GetInnerHeight();
    Task<int> GetInnerWidth();
    Task OnBrowserResize();
}

public class BrowserResizeService : IBrowserResizeService
{
    public event Func<Task>? OnResize;
    private readonly IJSRuntime _runtime;

    public BrowserResizeService(IJSRuntime runtime)
    {
        _runtime = runtime;
    }

    [JSInvokable]
    public async Task OnBrowserResize()
    {
        if (OnResize != null)
            await OnResize.Invoke();
    }

    public async Task<int> GetInnerHeight()
    {
        return await _runtime.InvokeAsync<int>("browserResize.getInnerHeight");
    }

    public async Task<int> GetInnerWidth()
    {
        return await _runtime.InvokeAsync<int>("browserResize.getInnerWidth");
    }
}
