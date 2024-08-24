namespace Kook.Commands;

/// <summary>
///     表示一个有关命令的结果的信息。
/// </summary>
public interface IResult
{
    /// <summary>
    ///     获取在命令执行期间发生的异常信息，如果没有异常则为 <c>null</c>。
    /// </summary>
    CommandError? Error { get; }

    /// <summary>
    ///     获取在命令执行期间发生的异常的原因，如果没有异常则为 <c>null</c>。
    /// </summary>
    string? ErrorReason { get; }

    /// <summary>
    ///     获取命令执行的结果是否为成功。
    /// </summary>
    bool IsSuccess { get; }
}
