namespace Kook;

/// <summary>
///     表示一个未知的内容过滤器处理器。
/// </summary>
public readonly struct ContentFilterUnknownHandler : IContentFilterHandler
{
    /// <summary>
    ///     获取处理器的类型。
    /// </summary>
    public ContentFilterHandlerType Type => 0;

    /// <inheritdoc />
    public bool Enabled { get; init; }

    /// <inheritdoc />
    public string? Name { get; init; }
}
