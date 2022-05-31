using System.Reactive.Subjects;

namespace SimpleCmsBlazor.Services;

public class ToastService
{
    public enum ToastState
    {
        Information,
        Success,
        Warning,
        Error
    }

    public struct ToastMessage
    {
        public string? Header { get;set; }
        public string? Body { get; set; }
        public ToastState State { get; set; }
        public int? Delay { get; set; }
    }

    private readonly Subject<ToastMessage> _toastSubject = new();

    public IObservable<ToastMessage> ToastSubject => _toastSubject;

    public void Post(ToastMessage message)
    {
        _toastSubject.OnNext(message);
    }
}
