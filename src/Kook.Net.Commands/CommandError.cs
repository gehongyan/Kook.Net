namespace Kook.Commands;

/// <summary>
///     表示命令执行的错误。
/// </summary>
public enum CommandError
{
    // 搜索

    /// <summary>
    ///     当命令未知时引发。
    /// </summary>
    UnknownCommand = 1,

    // 解析

    /// <summary>
    ///     当命令解析失败时引发。
    /// </summary>
    ParseFailed,

    /// <summary>
    ///     当输入文本的参数过少或过多时引发。
    /// </summary>
    BadArgCount,

    // 解析

    // CastFailed,

    /// <summary>
    ///     当对象未被 <see cref="TypeReader"/> 找到时引发。
    /// </summary>
    ObjectNotFound,

    /// <summary>
    ///     当 <see cref="TypeReader"/> 匹配到多个对象时引发。
    /// </summary>
    MultipleMatches,

    // 先决条件

    /// <summary>
    ///     当命令未满足先决条件时引发。
    /// </summary>
    UnmetPrecondition,

    // 执行

    /// <summary>
    ///     当命令执行时发生异常时引发。
    /// </summary>
    Exception,

    // 运行时

    /// <summary>
    ///     当命令在运行时未成功执行时引发。
    /// </summary>
    Unsuccessful
}
