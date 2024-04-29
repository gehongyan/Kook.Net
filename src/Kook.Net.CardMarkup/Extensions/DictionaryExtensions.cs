namespace Kook.CardMarkup.Extensions;

internal static class DictionaryExtensions
{
    public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV defaultValue) =>
        dictionary.TryGetValue(key, out TV? value) ? value : defaultValue;
}
