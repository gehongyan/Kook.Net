using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     表示一个命令的解析结果。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct ParseResult : IResult
{
    /// <summary>
    ///     获取所有实参解析结果。
    /// </summary>
    public IReadOnlyList<TypeReaderResult> ArgValues { get; }

    /// <summary>
    ///     获取所有形参解析结果。
    /// </summary>
    public IReadOnlyList<TypeReaderResult> ParamValues { get; }

    /// <inheritdoc/>
    public CommandError? Error { get; }

    /// <inheritdoc/>
    public string? ErrorReason { get; }

    /// <summary>
    ///     获取在解析过程中导致解析错误的参数信息。
    /// </summary>
    public ParameterInfo? ErrorParameter { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    private ParseResult(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues,
        CommandError? error, string? errorReason, ParameterInfo? errorParamInfo)
    {
        ArgValues = argValues;
        ParamValues = paramValues;
        Error = error;
        ErrorReason = errorReason;
        ErrorParameter = errorParamInfo;
    }

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="ParseResult"/> 结构的新实例，表示一个成功的解析。
    /// </summary>
    /// <param name="argValues"> 实参解析结果。 </param>
    /// <param name="paramValues"> 形参解析结果。 </param>
    /// <returns> 一个表示匹配成功的 <see cref="ParseResult"/>。 </returns>
    public static ParseResult FromSuccess(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues)
    {
        if (argValues.Any(x => x.Values.Count > 1))
            return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.", null);
        if (paramValues.Any(t => t.Values.Count > 1))
            return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.", null);
        return new ParseResult(argValues, paramValues, null, null, null);
    }

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="ParseResult"/> 结构的新实例，表示一个成功的解析。
    /// </summary>
    /// <param name="argValues"> 实参解析结果。 </param>
    /// <param name="paramValues"> 形参解析结果。 </param>
    /// <returns> 一个表示匹配成功的 <see cref="ParseResult"/>。 </returns>
    public static ParseResult FromSuccess(IReadOnlyList<TypeReaderValue> argValues, IReadOnlyList<TypeReaderValue> paramValues)
    {
        TypeReaderResult[] argList = [..argValues.Select(TypeReaderResult.FromSuccess)];
        TypeReaderResult[] paramList = [..paramValues.Select(TypeReaderResult.FromSuccess)];
        return new ParseResult(argList, paramList, null, null, null);
    }

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="ParseResult"/> 结构的新实例，表示一个失败的匹配。
    /// </summary>
    /// <param name="error"> 命令匹配失败的类型。 </param>
    /// <param name="reason"> 参数解析失败的原因。 </param>
    /// <returns> 一个表示解析失败的 <see cref="ParseResult"/>。 </returns>
    public static ParseResult FromError(CommandError error, string reason) =>
        new([], [], error, reason, null);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="ParseResult"/> 结构的新实例，表示一个失败的匹配。
    /// </summary>
    /// <param name="error"> 命令匹配失败的类型。 </param>
    /// <param name="reason"> 参数解析失败的原因。 </param>
    /// <param name="parameterInfo"> 导致解析失败的参数信息。 </param>
    /// <returns> 一个表示解析失败的 <see cref="ParseResult"/>。 </returns>
    public static ParseResult FromError(CommandError error, string reason, ParameterInfo parameterInfo) =>
        new([], [], error, reason, parameterInfo);

    /// <summary>
    ///     初始化一个包含指定异常的 <see cref="ParseResult"/> 结构的新实例，表示一个失败的匹配。
    /// </summary>
    /// <param name="ex"> 导致匹配失败的异常。 </param>
    /// <returns> 一个表示匹配失败的 <see cref="MatchResult"/>。 </returns>
    public static ParseResult FromError(Exception ex) => FromError(CommandError.Exception, ex.Message);

    /// <summary>
    ///     初始化一个包含指定异常的 <see cref="ParseResult"/> 结构的新实例，表示一个失败的匹配。
    /// </summary>
    /// <param name="result"> 失败的结果。 </param>
    /// <returns> 一个表示匹配失败的 <see cref="MatchResult"/>。 </returns>
    public static ParseResult FromError(IResult result) => new([], [], result.Error, result.ErrorReason, null);

    /// <summary>
    ///     初始化一个包含指定异常的 <see cref="ParseResult"/> 结构的新实例，表示一个失败的匹配。
    /// </summary>
    /// <param name="result"> 失败的结果。 </param>
    /// <param name="parameterInfo"> 导致解析失败的参数信息。 </param>
    /// <returns> 一个表示匹配失败的 <see cref="MatchResult"/>。 </returns>
    public static ParseResult FromError(IResult result, ParameterInfo parameterInfo) =>
        new([], [], result.Error, result.ErrorReason, parameterInfo);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess
        ? $"Success ({ArgValues.Count}{(ParamValues.Count > 0 ? $" +{ParamValues.Count} Values" : "")})"
        : $"{Error}: {ErrorReason}";
}
