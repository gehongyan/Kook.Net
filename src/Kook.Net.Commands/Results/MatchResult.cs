using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     表示一个命令的匹配结果。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class MatchResult : IResult
{
    /// <summary>
    ///     获取在命令执行期间可能与输入文本匹配的命令。
    /// </summary>
    public CommandMatch? Match { get; }

    /// <summary>
    ///     获取命令在执行工作流中的阶段。
    /// </summary>
    public IResult? Pipeline { get; }

    /// <inheritdoc />
    public CommandError? Error { get; }

    /// <inheritdoc />
    public string? ErrorReason { get; }

    /// <inheritdoc />
    public bool IsSuccess => !Error.HasValue;

    private MatchResult(CommandMatch? match, IResult? pipeline, CommandError? error, string? errorReason)
    {
        Match = match;
        Error = error;
        Pipeline = pipeline;
        ErrorReason = errorReason;
    }

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="MatchResult"/> 类的新实例，表示一个成功的匹配。
    /// </summary>
    /// <param name="match"> 命令匹配信息。 </param>
    /// <param name="pipeline"> 命令执行工作流中的阶段。 </param>
    /// <returns> 一个表示匹配成功的 <see cref="MatchResult"/>。 </returns>
    public static MatchResult FromSuccess(CommandMatch match, IResult pipeline) =>
        new(match, pipeline, null, null);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="MatchResult"/> 类的新实例，表示一个失败的匹配。
    /// </summary>
    /// <param name="error"> 命令匹配失败的类型。 </param>
    /// <param name="reason"> 命令匹配失败的原因。 </param>
    /// <returns> 一个表示匹配失败的 <see cref="MatchResult"/>。 </returns>
    public static MatchResult FromError(CommandError error, string reason) =>
        new(null, null, error, reason);

    /// <summary>
    ///     初始化一个包含指定异常的 <see cref="MatchResult"/> 类的新实例，表示一个失败的匹配。
    /// </summary>
    /// <param name="ex"> 导致匹配失败的异常。 </param>
    /// <returns> 一个表示匹配失败的 <see cref="MatchResult"/>。 </returns>
    public static MatchResult FromError(Exception ex) => FromError(CommandError.Exception, ex.Message);

    /// <summary>
    ///     初始化一个包含指定失败结果的 <see cref="MatchResult"/> 类的新实例，表示一个失败的匹配。
    /// </summary>
    /// <param name="result"> 失败的结果。 </param>
    /// <returns> 一个表示匹配失败的 <see cref="MatchResult"/>。 </returns>
    public static MatchResult FromError(IResult result) => new(null, null, result.Error, result.ErrorReason);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="MatchResult"/> 类的新实例，表示一个失败的匹配。
    /// </summary>
    /// <param name="pipeline"> 命令执行工作流中的阶段。 </param>
    /// <param name="error"> 错误类型。 </param>
    /// <param name="reason"> 错误原因。 </param>
    /// <returns> 一个表示匹配失败的 <see cref="MatchResult"/>。 </returns>
    public static MatchResult FromError(IResult pipeline, CommandError error, string reason) =>
        new(null, pipeline, error, reason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
}
