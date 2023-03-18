using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     Represents the result of a command search.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct SearchResult : IResult
{
    /// <summary>
    ///     Gets the text that was searched in.
    /// </summary>
    public string Text { get; }
    /// <summary>
    ///     Gets the commands that were found.
    /// </summary>
    public IReadOnlyList<CommandMatch> Commands { get; }

    /// <inheritdoc/>
    public CommandError? Error { get; }
    /// <inheritdoc/>
    public string ErrorReason { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    private SearchResult(string text, IReadOnlyList<CommandMatch> commands, CommandError? error, string errorReason)
    {
        Text = text;
        Commands = commands;
        Error = error;
        ErrorReason = errorReason;
    }

    /// <summary>
    ///     Returns a <see cref="SearchResult" /> with no errors.
    /// </summary>
    /// <param name="text"> The text that was searched in. </param>
    /// <param name="commands"> The commands that were found. </param>
    public static SearchResult FromSuccess(string text, IReadOnlyList<CommandMatch> commands)
        => new SearchResult(text, commands, null, null);
    /// <summary>
    ///     Returns a <see cref="SearchResult" /> with a <see cref="CommandError"/>.
    /// </summary>
    /// <param name="error"> The type of failure. </param>
    /// <param name="reason"> The reason of failure. </param>
    /// <returns></returns>
    public static SearchResult FromError(CommandError error, string reason)
        => new SearchResult(null, null, error, reason);
    /// <summary>
    ///     Returns a <see cref="SearchResult" /> with an exception.
    /// </summary>
    /// <param name="ex"> The exception that occurred. </param>
    public static SearchResult FromError(Exception ex)
        => FromError(CommandError.Exception, ex.Message);
    /// <summary>
    ///     Returns a <see cref="SearchResult" /> with the specified <paramref name="result"/> type.
    /// </summary>
    /// <param name="result"> The result of failure. </param>
    public static SearchResult FromError(IResult result)
        => new SearchResult(null, null, result.Error, result.ErrorReason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    private string DebuggerDisplay => IsSuccess ? $"Success ({Commands.Count} Results)" : $"{Error}: {ErrorReason}";
}
