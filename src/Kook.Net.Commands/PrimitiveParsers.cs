using System.Collections.Immutable;

namespace Kook.Commands;

internal delegate bool TryParseDelegate<T>(string str, out T value);

internal static class PrimitiveParsers
{
    private static readonly Lazy<IReadOnlyDictionary<Type, Delegate>> Parsers = new(CreateParsers);

    public static readonly IEnumerable<Type> SupportedTypes = Parsers.Value.Keys;

    private static IReadOnlyDictionary<Type, Delegate> CreateParsers()
    {
        ImmutableDictionary<Type, Delegate>.Builder parserBuilder = ImmutableDictionary.CreateBuilder<Type, Delegate>();
        parserBuilder[typeof(bool)] = (TryParseDelegate<bool>)bool.TryParse;
        parserBuilder[typeof(sbyte)] = (TryParseDelegate<sbyte>)sbyte.TryParse;
        parserBuilder[typeof(byte)] = (TryParseDelegate<byte>)byte.TryParse;
        parserBuilder[typeof(short)] = (TryParseDelegate<short>)short.TryParse;
        parserBuilder[typeof(ushort)] = (TryParseDelegate<ushort>)ushort.TryParse;
        parserBuilder[typeof(int)] = (TryParseDelegate<int>)int.TryParse;
        parserBuilder[typeof(uint)] = (TryParseDelegate<uint>)uint.TryParse;
        parserBuilder[typeof(long)] = (TryParseDelegate<long>)long.TryParse;
        parserBuilder[typeof(ulong)] = (TryParseDelegate<ulong>)ulong.TryParse;
        parserBuilder[typeof(float)] = (TryParseDelegate<float>)float.TryParse;
        parserBuilder[typeof(double)] = (TryParseDelegate<double>)double.TryParse;
        parserBuilder[typeof(decimal)] = (TryParseDelegate<decimal>)decimal.TryParse;
        parserBuilder[typeof(DateTime)] = (TryParseDelegate<DateTime>)DateTime.TryParse;
        parserBuilder[typeof(DateTimeOffset)] = (TryParseDelegate<DateTimeOffset>)DateTimeOffset.TryParse;
#if NET6_0_OR_GREATER
        parserBuilder[typeof(DateOnly)] = (TryParseDelegate<DateOnly>)DateOnly.TryParse;
        parserBuilder[typeof(TimeOnly)] = (TryParseDelegate<TimeOnly>)TimeOnly.TryParse;
#endif
        parserBuilder[typeof(char)] = (TryParseDelegate<char>)char.TryParse;
        return parserBuilder.ToImmutable();
    }

    public static TryParseDelegate<T> Get<T>() => (TryParseDelegate<T>)Parsers.Value[typeof(T)];
    public static Delegate Get(Type type) => Parsers.Value[type];
}
