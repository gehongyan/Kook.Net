using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Kook.Commands;

internal static class NullableTypeReader
{
    private static readonly FrozenDictionary<Type, Func<TypeReader, TypeReader>> PrimitiveNullableReaders =
        new Dictionary<Type, Func<TypeReader, TypeReader>>
            {
                [typeof(bool)] = x => new NullableTypeReader<bool>(x),
                [typeof(byte)] = x => new NullableTypeReader<byte>(x),
                [typeof(sbyte)] = x => new NullableTypeReader<sbyte>(x),
                [typeof(short)] = x => new NullableTypeReader<short>(x),
                [typeof(ushort)] = x => new NullableTypeReader<ushort>(x),
                [typeof(int)] = x => new NullableTypeReader<int>(x),
                [typeof(uint)] = x => new NullableTypeReader<uint>(x),
                [typeof(long)] = x => new NullableTypeReader<long>(x),
                [typeof(ulong)] = x => new NullableTypeReader<ulong>(x),
                [typeof(float)] = x => new NullableTypeReader<float>(x),
                [typeof(double)] = x => new NullableTypeReader<double>(x),
                [typeof(decimal)] = x => new NullableTypeReader<decimal>(x),
                [typeof(DateTime)] = x => new NullableTypeReader<DateTime>(x),
                [typeof(DateTimeOffset)] = x => new NullableTypeReader<DateTimeOffset>(x),
                [typeof(Guid)] = x => new NullableTypeReader<Guid>(x),
                [typeof(TimeSpan)] = x => new NullableTypeReader<TimeSpan>(x),
#if NET6_0_OR_GREATER
                [typeof(DateOnly)] = x => new NullableTypeReader<DateOnly>(x),
                [typeof(TimeOnly)] = x => new NullableTypeReader<TimeOnly>(x),
#endif
                [typeof(char)] = x => new NullableTypeReader<char>(x),
            }
            .ToFrozenDictionary();

    public static TypeReader Create(Type type, TypeReader reader)
    {
        if (PrimitiveNullableReaders.TryGetValue(type, out Func<TypeReader, TypeReader>? factory))
            return factory(reader);
        ConstructorInfo constructor = typeof(NullableTypeReader<>).MakeGenericType(type).GetTypeInfo().DeclaredConstructors.First();
        return (TypeReader)constructor.Invoke([reader]);
    }
}

#if NET6_0_OR_GREATER
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
internal class NullableTypeReader<T> : TypeReader
    where T : struct
{
    private readonly TypeReader _baseTypeReader;

    public NullableTypeReader(TypeReader baseTypeReader)
    {
        _baseTypeReader = baseTypeReader;
    }

    /// <inheritdoc />
    public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (string.Equals(input, "null", StringComparison.OrdinalIgnoreCase)
            || string.Equals(input, "nothing", StringComparison.OrdinalIgnoreCase))
            return TypeReaderResult.FromSuccess(new T( ));
        return await _baseTypeReader.ReadAsync(context, input, services).ConfigureAwait(false);
    }
}
