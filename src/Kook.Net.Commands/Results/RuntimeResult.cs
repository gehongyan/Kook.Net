using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     表示命令执行的运行时结果。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public abstract class RuntimeResult : IResult
{
    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="RuntimeResult" /> 类的新实例。
    /// </summary>
    /// <param name="error"> 错误类型。 </param>
    /// <param name="reason"> 错误原因。 </param>
    protected RuntimeResult(CommandError? error, string reason)
    {
        Error = error;
        Reason = reason;
    }

    /// <inheritdoc/>
    public CommandError? Error { get; }

    /// <summary>
    ///     获取错误原因。
    /// </summary>
    public string Reason { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    /// <inheritdoc/>
    string? IResult.ErrorReason => Reason;

    /// <inheritdoc />
    public override string ToString() => Reason ?? (IsSuccess ? "Successful" : "Unsuccessful");

    private string DebuggerDisplay => IsSuccess ? $"Success: {Reason ?? "No Reason"}" : $"{Error}: {Reason}";
}
