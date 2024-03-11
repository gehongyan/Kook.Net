namespace Kook.CardMarkup.Extensions;

internal static class DictionaryExtensions
{
    public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV defaultValue = default)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        return defaultValue;
    }
}
