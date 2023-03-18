using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     Represents a parsing result of a type reader.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct TypeReaderValue
{
    /// <summary>
    ///     Gets the parsed value.
    /// </summary>
    public object Value { get; }
    /// <summary>
    ///     Gets the confidence score of the parsing.
    /// </summary>
    public float Score { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeReaderValue"/> struct.
    /// </summary>
    /// <param name="value"> The parsed value. </param>
    /// <param name="score"> The confidence score of the parsing. </param>
    public TypeReaderValue(object value, float score)
    {
        Value = value;
        Score = score;
    }

    /// <inheritdoc />
    public override string ToString() => Value?.ToString();
    private string DebuggerDisplay => $"[{Value}, {Math.Round(Score, 2)}]";
}

/// <summary>
///     Represents a parsing result of a type reader.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct TypeReaderResult : IResult
{
    /// <summary>
    ///     Gets the parsed values.
    /// </summary>
    public IReadOnlyCollection<TypeReaderValue> Values { get; }

    /// <inheritdoc/>
    public CommandError? Error { get; }
    /// <inheritdoc/>
    public string ErrorReason { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    /// <exception cref="InvalidOperationException">TypeReaderResult was not successful.</exception>
    public object BestMatch => IsSuccess
#if NET5_0_OR_GREATER
        ? Values.Count == 1 ? Values.Single().Value : Values.MaxBy(v => v.Score).Value
#else
        ? Values.Count == 1 ? Values.Single().Value : Values.OrderByDescending(v => v.Score).First().Value
#endif
        : throw new InvalidOperationException("TypeReaderResult was not successful.");

    private TypeReaderResult(IReadOnlyCollection<TypeReaderValue> values, CommandError? error, string errorReason)
    {
        Values = values;
        Error = error;
        ErrorReason = errorReason;
    }

    /// <summary>
    ///     Returns a <see cref="TypeReaderResult" /> with no errors.
    /// </summary>
    /// <param name="value"> The parsed value. </param>
    public static TypeReaderResult FromSuccess(object value)
        => new TypeReaderResult(ImmutableArray.Create(new TypeReaderValue(value, 1.0f)), null, null);
    /// <summary>
    ///     Returns a <see cref="TypeReaderResult" /> with no errors.
    /// </summary>
    /// <param name="value"> The parsed value. </param>
    public static TypeReaderResult FromSuccess(TypeReaderValue value)
        => new TypeReaderResult(ImmutableArray.Create(value), null, null);
    /// <summary>
    ///     Returns a <see cref="TypeReaderResult" /> with no errors.
    /// </summary>
    /// <param name="values"> The parsed values. </param>
    public static TypeReaderResult FromSuccess(IReadOnlyCollection<TypeReaderValue> values)
        => new TypeReaderResult(values, null, null);
    /// <summary>
    ///     Returns a <see cref="TypeReaderResult" /> with a specified error.
    /// </summary>
    /// <param name="error"> The error. </param>
    /// <param name="reason"> The reason for the error. </param>
    public static TypeReaderResult FromError(CommandError error, string reason)
        => new TypeReaderResult(null, error, reason);
    /// <summary>
    ///     Returns a <see cref="TypeReaderResult" /> with an exception.
    /// </summary>
    /// <param name="ex"> The exception that occurred. </param>
    public static TypeReaderResult FromError(Exception ex)
        => FromError(CommandError.Exception, ex.Message);
    /// <summary>
    ///     Returns a <see cref="TypeReaderResult" /> with an specified result.
    /// </summary>
    /// <param name="result"> The result. </param>
    public static TypeReaderResult FromError(IResult result)
        => new TypeReaderResult(null, result.Error, result.ErrorReason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    private string DebuggerDisplay => IsSuccess ? $"Success ({string.Join(", ", Values)})" : $"{Error}: {ErrorReason}";
}
