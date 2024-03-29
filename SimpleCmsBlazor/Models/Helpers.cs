namespace SimpleCmsBlazor.Models;

public static class Helpers
{
    public static T GetPropValue<T>(this object obj, string name)
    {
        var prop = obj.GetType().GetProperty(name) ?? throw new HelperException($"Property {name} does not exist");
        return (T)(prop.GetValue(obj) ?? new());
    }

    public static void SetPropValue(this object obj, string name, object value)
    {
        var prop = obj.GetType().GetProperty(name) ?? throw new HelperException("Property does not exist");
        prop.SetValue(obj, value);
    }
}

public class HelperException : Exception
{
    public HelperException()
    {
    }

    public HelperException(string? message) : base(message)
    {
    }

    public HelperException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}