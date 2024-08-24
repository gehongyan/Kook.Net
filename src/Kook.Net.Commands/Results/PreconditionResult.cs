using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     表示一个命令的先决条件检查结果。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PreconditionResult : IResult
{
    /// <inheritdoc/>
    public CommandError? Error { get; }

    /// <inheritdoc/>
    public string? ErrorReason { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="PreconditionResult"/> 类的新实例。
    /// </summary>
    /// <param name="error"> 错误类型。 </param>
    /// <param name="errorReason"> 错误原因。 </param>
    protected PreconditionResult(CommandError? error, string? errorReason)
    {
        Error = error;
        ErrorReason = errorReason;
    }

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="PreconditionResult"/> 类的新实例，表示一个成功的先决条件检查。
    /// </summary>
    /// <returns> 一个表示先决条件检查成功的 <see cref="PreconditionResult"/>。 </returns>
    public static PreconditionResult FromSuccess() => new(null, null);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="PreconditionResult"/> 类的新实例，表示一个失败的先决条件检查。
    /// </summary>
    /// <param name="reason"> 错误原因。 </param>
    /// <returns> 一个表示先决条件检查失败的 <see cref="PreconditionResult"/>。 </returns>
    public static PreconditionResult FromError(string reason) => new(CommandError.UnmetPrecondition, reason);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="PreconditionResult"/> 类的新实例，表示一个失败的先决条件检查。
    /// </summary>
    /// <param name="ex"> 导致先决条件检查失败的异常。 </param>
    /// <returns> 一个表示先决条件检查失败的 <see cref="PreconditionResult"/>。 </returns>
    public static PreconditionResult FromError(Exception ex) => new(CommandError.Exception, ex.Message);

    /// <summary>
    ///     初始化一个包含指定结果的 <see cref="PreconditionResult"/> 类的新实例，表示一个失败的先决条件检查。
    /// </summary>
    /// <param name="result"> 要包装的结果。 </param>
    /// <returns> 一个表示先决条件检查失败的 <see cref="PreconditionResult"/>。 </returns>
    public static PreconditionResult FromError(IResult result) => new(result.Error, result.ErrorReason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
}
