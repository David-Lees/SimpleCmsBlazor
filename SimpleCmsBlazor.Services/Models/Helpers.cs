namespace SimpleCmsBlazor.Models;

public static class Helpers
{
    public static T GetPropValue<T>(this object obj, string name)
    {
        var prop = obj.GetType().GetProperty(name) ?? throw new NullReferenceException("Property does not exist");
        return (T)(prop.GetValue(obj) ?? new());
    }

    public static void SetPropValue(this object obj, string name, object value)
    {
        var prop = obj.GetType().GetProperty(name) ?? throw new NullReferenceException("Property does not exist");
        prop.SetValue(obj, value);
    }
}
