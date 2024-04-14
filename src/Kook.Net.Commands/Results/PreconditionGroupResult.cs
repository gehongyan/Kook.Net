using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     Represents the result of a grouped precondition check.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PreconditionGroupResult : PreconditionResult
{
    /// <summary>
    ///     Gets the results of the precondition checks.
    /// </summary>
    public IReadOnlyCollection<PreconditionResult> PreconditionResults { get; }

    /// <summary>
    ///     Creates a new <see cref="PreconditionGroupResult"/> with the specified error, reason, and precondition results.
    /// </summary>
    /// <param name="error"> The error that occurred. </param>
    /// <param name="errorReason"> The reason for the error. </param>
    /// <param name="preconditions"> The results of the precondition checks. </param>
    protected PreconditionGroupResult(CommandError? error, string errorReason, ICollection<PreconditionResult> preconditions)
        : base(error, errorReason) =>
        PreconditionResults = (preconditions ?? new List<PreconditionResult>(0)).ToReadOnlyCollection();

    /// <summary>
    ///     Returns a <see cref="PreconditionResult" /> with no errors.
    /// </summary>
    public static new PreconditionGroupResult FromSuccess()
        => new(null, null, null);

    /// <summary>
    ///     Returns a <see cref="PreconditionResult" /> with the reason and precondition results.
    /// </summary>
    /// <param name="reason"> The reason for the error. </param>
    /// <param name="preconditions"> The results of the precondition checks. </param>
    public static PreconditionGroupResult FromError(string reason, ICollection<PreconditionResult> preconditions)
        => new(CommandError.UnmetPrecondition, reason, preconditions);

    /// <summary>
    ///     Returns a <see cref="PreconditionResult" /> with an exception.
    /// </summary>
    /// <param name="ex"> The exception that occurred. </param>
    public static new PreconditionGroupResult FromError(Exception ex)
        => new(CommandError.Exception, ex.Message, null);

    /// <summary>
    ///     Returns a <see cref="PreconditionResult" /> with the specified result.
    /// </summary>
    /// <param name="result"> The result of failure. </param>
    public static new PreconditionGroupResult FromError(IResult result) //needed?
        => new(result.Error, result.ErrorReason, null);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
}
