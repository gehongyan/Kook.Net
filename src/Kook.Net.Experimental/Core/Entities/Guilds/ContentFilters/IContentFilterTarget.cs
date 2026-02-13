namespace Kook;

/// <summary>
///     表示一个服务器内容过滤目标。
/// </summary>
public interface IContentFilterTarget
{
    /// <summary>
    ///     获取此内容过滤器的类型。
    /// </summary>
    ContentFilterType Type { get; }

    /// <summary>
    ///     获取此内容过滤目标的模式。
    /// </summary>
    ContentFilterMode Mode { get; }
}