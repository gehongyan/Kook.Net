using System.Collections.Immutable;
using System.Reflection;

namespace Kook.Commands;

internal static class EnumTypeReader
{
    public static TypeReader GetReader(Type type)
    {
        if (!type.IsEnum) throw new ArgumentException("Type must be an enum.", nameof(type));
        Type baseType = Enum.GetUnderlyingType(type);
        ConstructorInfo constructor = typeof(EnumTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
        return (TypeReader) constructor.Invoke([type, PrimitiveParsers.Get(baseType)]);
    }
}

internal class EnumTypeReader<T> : TypeReader
    where T : struct, Enum
{
    private readonly IReadOnlyDictionary<string, object> _enumsByName;
    private readonly IReadOnlyDictionary<T, object> _enumsByValue;
    private readonly Type _enumType;
    private readonly TryParseDelegate<T> _tryParse;

    public EnumTypeReader(Type type, TryParseDelegate<T> parser)
    {
        _enumType = type;
        _tryParse = parser;

        ImmutableDictionary<string, object>.Builder byNameBuilder = ImmutableDictionary.CreateBuilder<string, object>();
        ImmutableDictionary<T, object>.Builder byValueBuilder = ImmutableDictionary.CreateBuilder<T, object>();

        foreach (string v in Enum.GetNames(_enumType))
        {
            object parsedValue = Enum.Parse(_enumType, v);
            byNameBuilder.Add(v.ToLower(), parsedValue);
            if (!byValueBuilder.ContainsKey((T)parsedValue)) byValueBuilder.Add((T)parsedValue, parsedValue);
        }

        _enumsByName = byNameBuilder.ToImmutable();
        _enumsByValue = byValueBuilder.ToImmutable();
    }

    /// <inheritdoc />
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        object? enumValue;

        if (_tryParse(input, out T baseValue))
        {
            return Task.FromResult(_enumsByValue.TryGetValue(baseValue, out enumValue)
                ? TypeReaderResult.FromSuccess(enumValue)
                : TypeReaderResult.FromError(CommandError.ParseFailed, $"Value is not a {_enumType.Name}."));
        }

        return Task.FromResult(_enumsByName.TryGetValue(input.ToLower(), out enumValue)
            ? TypeReaderResult.FromSuccess(enumValue)
            : TypeReaderResult.FromError(CommandError.ParseFailed, $"Value is not a {_enumType.Name}."));
    }
}
