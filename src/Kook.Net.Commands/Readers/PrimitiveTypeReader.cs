using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace Kook.Commands;

internal static class PrimitiveTypeReader
{
    private static readonly FrozenDictionary<Type, Func<TypeReader>> PrimitiveReaders =
        new Dictionary<Type, Func<TypeReader>>
            {
                [typeof(bool)] = () => new PrimitiveTypeReader<bool>(),
                [typeof(byte)] = () => new PrimitiveTypeReader<byte>(),
                [typeof(sbyte)] = () => new PrimitiveTypeReader<sbyte>(),
                [typeof(short)] = () => new PrimitiveTypeReader<short>(),
                [typeof(ushort)] = () => new PrimitiveTypeReader<ushort>(),
                [typeof(int)] = () => new PrimitiveTypeReader<int>(),
                [typeof(uint)] = () => new PrimitiveTypeReader<uint>(),
                [typeof(long)] = () => new PrimitiveTypeReader<long>(),
                [typeof(ulong)] = () => new PrimitiveTypeReader<ulong>(),
                [typeof(float)] = () => new PrimitiveTypeReader<float>(),
                [typeof(double)] = () => new PrimitiveTypeReader<double>(),
                [typeof(decimal)] = () => new PrimitiveTypeReader<decimal>(),
                [typeof(DateTime)] = () => new PrimitiveTypeReader<DateTime>(),
                [typeof(DateTimeOffset)] = () => new PrimitiveTypeReader<DateTimeOffset>(),
                [typeof(Guid)] = () => new PrimitiveTypeReader<Guid>(),
#if NET6_0_OR_GREATER
                [typeof(DateOnly)] = () => new PrimitiveTypeReader<DateOnly>(),
                [typeof(TimeOnly)] = () => new PrimitiveTypeReader<TimeOnly>(),
#endif
                [typeof(char)] = () => new PrimitiveTypeReader<char>(),
            }
            .ToFrozenDictionary();

    public static TypeReader? Create(Type type)
    {
        if (PrimitiveReaders.TryGetValue(type, out Func<TypeReader>? factory))
            return factory();
        type = typeof(PrimitiveTypeReader<>).MakeGenericType(type);
        return Activator.CreateInstance(type) as TypeReader;
    }
}

#if NET6_0_OR_GREATER
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
internal class PrimitiveTypeReader<T> : TypeReader
{
    private readonly TryParseDelegate<T> _tryParse;
    private readonly float _score;

    /// <exception cref="ArgumentOutOfRangeException"><typeparamref name="T"/> must be within the range [0, 1].</exception>
    public PrimitiveTypeReader()
        : this(PrimitiveParsers.Get<T>(), 1)
    {
    }

    /// <exception cref="ArgumentOutOfRangeException"><paramref name="score"/> must be within the range [0, 1].</exception>
    public PrimitiveTypeReader(TryParseDelegate<T> tryParse, float score)
    {
        if (score is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(score), score, "Scores must be within the range [0, 1].");
        _tryParse = tryParse;
        _score = score;
    }

    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (_tryParse(input, out T value))
            return Task.FromResult(TypeReaderResult.FromSuccess(new TypeReaderValue(value, _score)));
        return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {typeof(T).Name}."));
    }
}
