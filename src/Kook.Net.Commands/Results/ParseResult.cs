using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     Contains information for the parsing result from the command service's parser.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct ParseResult : IResult
{
    /// <summary>
    ///     Gets a read-only collection containing the parsed argument values.
    /// </summary>
    public IReadOnlyList<TypeReaderResult> ArgValues { get; }

    /// <summary>
    ///     Gets a read-only collection containing the parsed parameter values.
    /// </summary>
    public IReadOnlyList<TypeReaderResult> ParamValues { get; }

    /// <inheritdoc/>
    public CommandError? Error { get; }

    /// <inheritdoc/>
    public string? ErrorReason { get; }

    /// <summary>
    ///     Provides information about the parameter that caused the parsing error.
    /// </summary>
    /// <returns>
    ///     A <see cref="ParameterInfo" /> indicating the parameter info of the error that may have occurred during parsing;
    ///     <c>null</c> if the parsing was successful or the parsing error is not specific to a single parameter.
    /// </returns>
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
    ///     Creates a successful parsing result.
    /// </summary>
    /// <param name="argValues"> The parsed argument values. </param>
    /// <param name="paramValues"> The parsed parameter values. </param>
    /// <returns> The parsing result. </returns>
    public static ParseResult FromSuccess(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues)
    {
        if (argValues.Any(x => x.Values.Count > 1))
            return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.", null);
        if (paramValues.Any(t => t.Values.Count > 1))
            return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.", null);
        return new ParseResult(argValues, paramValues, null, null, null);
    }

    /// <summary>
    ///     Creates a successful parsing result.
    /// </summary>
    /// <param name="argValues"> The parsed argument values. </param>
    /// <param name="paramValues"> The parsed parameter values. </param>
    /// <returns> The parsing result. </returns>
    public static ParseResult FromSuccess(IReadOnlyList<TypeReaderValue> argValues, IReadOnlyList<TypeReaderValue> paramValues)
    {
        TypeReaderResult[] argList = [..argValues.Select(TypeReaderResult.FromSuccess)];
        TypeReaderResult[] paramList = [..paramValues.Select(TypeReaderResult.FromSuccess)];
        return new ParseResult(argList, paramList, null, null, null);
    }

    /// <summary>
    ///     Creates a failed parsing result.
    /// </summary>
    /// <param name="error"> The error that occurred. </param>
    /// <param name="reason"> The reason for the error. </param>
    /// <returns> The parsing result. </returns>
    public static ParseResult FromError(CommandError error, string reason) =>
        new([], [], error, reason, null);

    /// <summary>
    ///     Creates a failed parsing result.
    /// </summary>
    /// <param name="error"> The error that occurred. </param>
    /// <param name="reason"> The reason for the error. </param>
    /// <param name="parameterInfo"> The parameter info of the error that may have occurred during parsing. </param>
    /// <returns> The parsing result. </returns>
    public static ParseResult FromError(CommandError error, string reason, ParameterInfo parameterInfo) =>
        new([], [], error, reason, parameterInfo);

    /// <summary>
    ///     Creates a failed parsing result.
    /// </summary>
    /// <param name="ex"> The exception that occurred. </param>
    /// <returns> The parsing result. </returns>
    public static ParseResult FromError(Exception ex) => FromError(CommandError.Exception, ex.Message);

    /// <summary>
    ///     Creates a failed parsing result.
    /// </summary>
    /// <param name="result"> The result that contains the error. </param>
    /// <returns> The parsing result. </returns>
    public static ParseResult FromError(IResult result) => new([], [], result.Error, result.ErrorReason, null);

    /// <summary>
    ///     Creates a failed parsing result.
    /// </summary>
    /// <param name="result"> The result that contains the error. </param>
    /// <param name="parameterInfo"> The parameter info of the error that may have occurred during parsing. </param>
    /// <returns> The parsing result. </returns>
    public static ParseResult FromError(IResult result, ParameterInfo parameterInfo) =>
        new([], [], result.Error, result.ErrorReason, parameterInfo);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess
        ? $"Success ({ArgValues.Count}{(ParamValues.Count > 0 ? $" +{ParamValues.Count} Values" : "")})"
        : $"{Error}: {ErrorReason}";
}
