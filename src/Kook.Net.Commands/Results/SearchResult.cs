using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     表示一个命令搜索的结果。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct SearchResult : IResult
{
    /// <summary>
    ///     获取用于搜索的文本。
    /// </summary>
    public string? Text { get; }

    /// <summary>
    ///     获取所有搜索到的命令。
    /// </summary>
    public IReadOnlyList<CommandMatch> Commands { get; }

    /// <inheritdoc/>
    public CommandError? Error { get; }

    /// <inheritdoc/>
    public string? ErrorReason { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    private SearchResult(string? text, IReadOnlyList<CommandMatch> commands, CommandError? error, string? errorReason)
    {
        Text = text;
        Commands = commands;
        Error = error;
        ErrorReason = errorReason;
    }

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="SearchResult"/> 结构的新实例，表示一个成功的搜索。
    /// </summary>
    /// <param name="text"> 用于搜索的文本。 </param>
    /// <param name="commands"> 搜索到的命令。 </param>
    /// <returns> 一个表示搜索成功的 <see cref="SearchResult"/>。 </returns>
    public static SearchResult FromSuccess(string text, IReadOnlyList<CommandMatch> commands) =>
        new(text, commands, null, null);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="SearchResult"/> 结构的新实例，表示一个失败的搜索。
    /// </summary>
    /// <param name="error"> 搜索失败的类型。 </param>
    /// <param name="reason"> 搜索失败的原因。 </param>
    /// <returns> 一个表示搜索失败的 <see cref="SearchResult"/>。 </returns>
    public static SearchResult FromError(CommandError error, string reason) => new(null, [], error, reason);

    /// <summary>
    ///     初始化一个包含指定异常的 <see cref="SearchResult"/> 结构的新实例，表示一个失败的搜索。
    /// </summary>
    /// <param name="ex"> 导致搜索失败的异常。 </param>
    /// <returns> 一个表示搜索失败的 <see cref="SearchResult"/>。 </returns>
    public static SearchResult FromError(Exception ex) => FromError(CommandError.Exception, ex.Message);

    /// <summary>
    ///     初始化一个包含指定结果的 <see cref="SearchResult"/> 结构的新实例，表示一个失败的搜索。
    /// </summary>
    /// <param name="result"> 失败的结果。 </param>
    /// <returns> 一个表示搜索失败的 <see cref="SearchResult"/>。 </returns>
    public static SearchResult FromError(IResult result) => new(null, [], result.Error, result.ErrorReason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess ? $"Success ({Commands?.Count ?? 0} Results)" : $"{Error}: {ErrorReason}";
}
