namespace Kook;

internal static class ValueHelper
{
    public static bool SetIfChanged<T>(Func<T> getter, Action<T> setter, T value, Func<T, T, bool>? comparer = null)
    {
        T oldValue = getter();
        bool equals = comparer?.Invoke(oldValue, value)
            ?? EqualityComparer<T>.Default.Equals(oldValue, value);
        if (equals) return false;
        setter(value);
        return true;
    }
}
