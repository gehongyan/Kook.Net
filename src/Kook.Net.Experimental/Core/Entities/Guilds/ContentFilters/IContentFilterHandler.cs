namespace Kook;

/// <summary>
///     表示一个内容过滤器处理器。
/// </summary>
public interface IContentFilterHandler
{
    /// <summary>
    ///     获取处理器的类型。
    /// </summary>
    ContentFilterHandlerType Type { get; }

    /// <summary>
    ///     获取处理器是否启用。
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    ///     获取处理器的名称。
    /// </summary>
    string? Name { get; }
}
