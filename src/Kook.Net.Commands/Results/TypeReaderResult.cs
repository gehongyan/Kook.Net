using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     表示一个类型读取器的解析值。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct TypeReaderValue
{
    /// <summary>
    ///     获取解析的值。
    /// </summary>
    public object? Value { get; }

    /// <summary>
    ///     获取解析的置信度分数。
    /// </summary>
    public float Score { get; }

    /// <summary>
    ///     齿梳化一个包含解析值和置信度分数的 <see cref="TypeReaderValue"/> 结构的新实例。
    /// </summary>
    /// <param name="value"> 解析的值。 </param>
    /// <param name="score"> 解析的置信度分数。 </param>
    public TypeReaderValue(object? value, float score)
    {
        Value = value;
        Score = score;
    }

    /// <inheritdoc />
    public override string? ToString() => Value?.ToString();

    private string DebuggerDisplay => $"[{Value}, {Math.Round(Score, 2)}]";
}

/// <summary>
///     表示一个类型读取器的解析结果。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct TypeReaderResult : IResult
{
    /// <summary>
    ///     获取解析的值。
    /// </summary>
    public IReadOnlyCollection<TypeReaderValue> Values { get; }

    /// <inheritdoc/>
    public CommandError? Error { get; }

    /// <inheritdoc/>
    public string? ErrorReason { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    /// <summary>
    ///     获取最佳匹配的解析值。
    /// </summary>
    /// <exception cref="InvalidOperationException"> 解析失败。 </exception>
    public object? BestMatch => IsSuccess
        ? Values?.MaxBy(v => v.Score).Value
        : throw new InvalidOperationException("TypeReaderResult was not successful.");

    private TypeReaderResult(IReadOnlyCollection<TypeReaderValue> values, CommandError? error, string? errorReason)
    {
        Values = values;
        Error = error;
        ErrorReason = errorReason;
    }

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="TypeReaderResult"/> 结构的新实例，表示一个成功的解析。
    /// </summary>
    /// <param name="value"> 解析的值。 </param>
    /// <returns> 一个表示解析成功的 <see cref="TypeReaderResult"/>。 </returns>
    public static TypeReaderResult FromSuccess(object? value) =>
        new(ImmutableArray.Create(new TypeReaderValue(value, 1.0f)), null, null);

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="TypeReaderResult"/> 结构的新实例，表示一个成功的解析。
    /// </summary>
    /// <param name="value"> 解析的值。 </param>
    /// <returns> 一个表示解析成功的 <see cref="TypeReaderResult"/>。 </returns>
    public static TypeReaderResult FromSuccess(TypeReaderValue value) =>
        new(ImmutableArray.Create(value), null, null);

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="TypeReaderResult"/> 结构的新实例，表示一个成功的解析。
    /// </summary>
    /// <param name="values"> 解析的值。 </param>
    /// <returns> 一个表示解析成功的 <see cref="TypeReaderResult"/>。 </returns>
    public static TypeReaderResult FromSuccess(IReadOnlyCollection<TypeReaderValue> values) => new(values, null, null);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="TypeReaderResult"/> 结构的新实例，表示一个失败的解析。
    /// </summary>
    /// <param name="error"> 错误类型。 </param>
    /// <param name="reason"> 错误原因。 </param>
    /// <returns> 一个表示解析失败的 <see cref="TypeReaderResult"/>。 </returns>
    public static TypeReaderResult FromError(CommandError error, string reason) => new([], error, reason);

    /// <summary>
    ///     初始化一个包含指定异常的 <see cref="TypeReaderResult"/> 结构的新实例，表示一个失败的解析。
    /// </summary>
    /// <param name="ex"> 导致解析失败的异常。 </param>
    /// <returns> 一个表示解析失败的 <see cref="TypeReaderResult"/>。 </returns>
    public static TypeReaderResult FromError(Exception ex) => FromError(CommandError.Exception, ex.Message);

    /// <summary>
    ///     初始化一个包含指定结果的 <see cref="TypeReaderResult"/> 结构的新实例，表示一个失败的解析。
    /// </summary>
    /// <param name="result"> 要包装的结果。 </param>
    /// <returns> 一个表示解析失败的 <see cref="TypeReaderResult"/>。 </returns>
    public static TypeReaderResult FromError(IResult result) => new([], result.Error, result.ErrorReason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess ? $"Success ({string.Join(", ", Values ?? [])})" : $"{Error}: {ErrorReason}";
}
