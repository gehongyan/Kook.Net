namespace Kook.Commands;

/// <summary>
///     表示命令执行工作流的行为。
/// </summary>
public enum RunMode
{
    /// <summary>
    ///     使用在 <see cref="T:Kook.Commands.CommandServiceConfig"/> 中设置的默认行为。
    /// </summary>
    Default,

    /// <summary>
    ///     在与网关线程相同的线程上执行命令。
    /// </summary>
    Sync,

    /// <summary>
    ///     在与网关线程不同的线程上执行命令。
    /// </summary>
    Async
}
