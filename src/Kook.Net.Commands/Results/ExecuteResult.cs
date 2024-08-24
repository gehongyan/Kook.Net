using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     表示一个命令的整体执行结果的信息。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct ExecuteResult : IResult
{
    /// <summary>
    ///     获取在命令执行期间发生的异常，如果没有异常则为 <c>null</c>。
    /// </summary>
    public Exception? Exception { get; }

    /// <inheritdoc />
    public CommandError? Error { get; }

    /// <inheritdoc />
    public string? ErrorReason { get; }

    /// <inheritdoc />
    public bool IsSuccess => !Error.HasValue;

    private ExecuteResult(Exception? exception, CommandError? error, string? errorReason)
    {
        Exception = exception;
        Error = error;
        ErrorReason = errorReason;
    }

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="ExecuteResult"/> 结构的新实例，表示一个成功的执行。
    /// </summary>
    /// <returns> 一个表示执行成功的 <see cref="ExecuteResult"/>。 </returns>
    public static ExecuteResult FromSuccess() => new(null, null, null);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="ExecuteResult"/> 结构的新实例，表示一个失败的执行。
    /// </summary>
    /// <param name="error"> 错误类型。 </param>
    /// <param name="reason"> 错误原因。 </param>
    /// <returns> 一个表示执行失败的 <see cref="ExecuteResult"/>。 </returns>
    public static ExecuteResult FromError(CommandError error, string reason) => new(null, error, reason);

    /// <summary>
    ///     初始化一个包含指定异常的 <see cref="ExecuteResult"/> 结构的新实例，表示一个失败的执行。
    /// </summary>
    /// <param name="ex"> 导致执行失败的异常。 </param>
    /// <returns>
    ///     一个包含导致执行失败的异常的 <see cref="ExecuteResult"/>，其错误类型为
    ///     <see cref="F:Kook.Commands.CommandError.Exception"/>，原因为异常消息。
    /// </returns>
    public static ExecuteResult FromError(Exception? ex) => new(ex, CommandError.Exception, ex?.Message);

    /// <summary>
    ///     初始化一个包含指定结果的 <see cref="ExecuteResult"/> 结构的新实例，根据指定的 <see cref="Kook.Commands.IResult.Error"/> 和
    ///     <see cref="Kook.Commands.IResult.ErrorReason"/>，这可能是一个成功的执行，也可能是一个失败的执行。
    /// </summary>
    /// <param name="result"> 要包装的结果。 </param>
    /// <returns> 一个表示执行结果的 <see cref="ExecuteResult"/>，错误类型和原因与 <paramref name="result"/> 相同。 </returns>
    public static ExecuteResult FromError(IResult result) => new(null, result.Error, result.ErrorReason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
}
