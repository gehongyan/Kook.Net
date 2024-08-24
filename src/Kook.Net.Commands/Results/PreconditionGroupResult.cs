using System.Diagnostics;

namespace Kook.Commands;

/// <summary>
///     表示一个分组的先决条件检查结果。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PreconditionGroupResult : PreconditionResult
{
    /// <summary>
    ///     获取先决条件检查的结果。
    /// </summary>
    public IReadOnlyCollection<PreconditionResult> PreconditionResults { get; }

    /// <summary>
    ///     初始化一个包含指定错误、原因和先决条件检查结果的 <see cref="PreconditionGroupResult"/> 类的新实例。
    /// </summary>
    /// <param name="error"> 错误类型。 </param>
    /// <param name="errorReason"> 错误原因。 </param>
    /// <param name="preconditions"> 先决条件检查结果。 </param>
    protected PreconditionGroupResult(CommandError? error, string? errorReason, ICollection<PreconditionResult> preconditions)
        : base(error, errorReason)
    {
        PreconditionResults = [..preconditions];
    }

    /// <summary>
    ///     初始化一个不包含任何错误的 <see cref="PreconditionGroupResult"/> 类的新实例，表示一个成功的先决条件检查。
    /// </summary>
    /// <returns> 一个表示先决条件检查成功的 <see cref="PreconditionGroupResult"/>。 </returns>
    public static new PreconditionGroupResult FromSuccess() => new(null, null, []);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="PreconditionGroupResult"/> 类的新实例，表示一个失败的先决条件检查。
    /// </summary>
    /// <param name="reason"> 错误原因。 </param>
    /// <param name="preconditions"> 先决条件检查结果。 </param>
    /// <returns> 一个表示先决条件检查失败的 <see cref="PreconditionGroupResult"/>。 </returns>
    public static PreconditionGroupResult FromError(string reason, ICollection<PreconditionResult> preconditions) =>
        new(CommandError.UnmetPrecondition, reason, preconditions);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="PreconditionGroupResult"/> 类的新实例，表示一个失败的先决条件检查。
    /// </summary>
    /// <param name="ex"> 导致先决条件检查失败的异常。 </param>
    /// <returns> 一个表示先决条件检查失败的 <see cref="PreconditionGroupResult"/>。 </returns>
    public static new PreconditionGroupResult FromError(Exception ex) => new(CommandError.Exception, ex.Message, []);

    /// <summary>
    ///     初始化一个包含指定错误类型和原因的 <see cref="PreconditionGroupResult"/> 类的新实例，表示一个失败的先决条件检查。
    /// </summary>
    /// <param name="result"> 包含错误类型和原因的结果。 </param>
    /// <returns> 一个表示先决条件检查失败的 <see cref="PreconditionGroupResult"/>。 </returns>
    public static new PreconditionGroupResult FromError(IResult result) => new(result.Error, result.ErrorReason, []); //needed?

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
}
