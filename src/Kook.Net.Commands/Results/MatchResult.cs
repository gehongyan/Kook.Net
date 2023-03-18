using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     Represents the match result of a command.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class MatchResult : IResult
{
    /// <summary>
    ///     Gets the command that may have matched during the command execution.
    /// </summary>
    public CommandMatch? Match { get; }

    /// <summary>
    ///     Gets on which pipeline stage the command may have matched or failed.
    /// </summary>
    public IResult Pipeline { get; }

    /// <inheritdoc />
    public CommandError? Error { get; }

    /// <inheritdoc />
    public string ErrorReason { get; }

    /// <inheritdoc />
    public bool IsSuccess => !Error.HasValue;

    private MatchResult(CommandMatch? match, IResult pipeline, CommandError? error, string errorReason)
    {
        Match = match;
        Error = error;
        Pipeline = pipeline;
        ErrorReason = errorReason;
    }

    /// <summary>
    ///     Creates a successful match result.
    /// </summary>
    /// <param name="match"> The command that matched. </param>
    /// <param name="pipeline"> The pipeline stage on which the command matched. </param>
    /// <returns> The match result. </returns>
    public static MatchResult FromSuccess(CommandMatch match, IResult pipeline)
        => new(match, pipeline, null, null);

    /// <summary>
    ///     Creates a failed match result.
    /// </summary>
    /// <param name="error"> The error that occurred. </param>
    /// <param name="reason"> The reason for the error. </param>
    /// <returns> The match result. </returns>
    public static MatchResult FromError(CommandError error, string reason)
        => new(null, null, error, reason);

    /// <summary>
    ///     Creates a failed match result.
    /// </summary>
    /// <param name="ex"> The exception that occurred. </param>
    /// <returns> The match result. </returns>
    public static MatchResult FromError(Exception ex)
        => FromError(CommandError.Exception, ex.Message);

    /// <summary>
    ///     Creates a failed match result.
    /// </summary>
    /// <param name="result"> The result that failed. </param>
    /// <returns> The match result. </returns>
    public static MatchResult FromError(IResult result)
        => new(null, null, result.Error, result.ErrorReason);

    /// <summary>
    ///     Creates a failed match result.
    /// </summary>
    /// <param name="pipeline"> The pipeline stage on which the command failed. </param>
    /// <param name="error"> The error that occurred. </param>
    /// <param name="reason"> The reason for the error. </param>
    /// <returns> The match result. </returns>
    public static MatchResult FromError(IResult pipeline, CommandError error, string reason)
        => new(null, pipeline, error, reason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
}
