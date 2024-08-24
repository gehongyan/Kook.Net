namespace Kook.Commands;

/// <summary>
///     表示在命令解析阶段匹配到多个结果时的行为。
/// </summary>
public enum MultiMatchHandling
{
    /// <summary>
    ///     当匹配到多个结果时引发异常。
    /// </summary>
    Exception,

    /// <summary>
    ///     当匹配到多个结果时选择最佳匹配。
    /// </summary>
    Best
}
