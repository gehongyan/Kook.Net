using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个内容过滤器拦截处理器。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct ContentFilterInterceptHandler : IContentFilterHandler
{
    /// <summary>
    ///     获取处理器的类型。
    /// </summary>
    public ContentFilterHandlerType Type => ContentFilterHandlerType.Intercept;

    /// <inheritdoc />
    public bool Enabled { get; init; }

    /// <inheritdoc />
    public string? Name { get; init; }

    /// <summary>
    ///     获取自定义的错误消息。
    /// </summary>
    public string? CustomErrorMessage { get; init; }

    private string DebuggerDisplay => $"Intercept{(!string.IsNullOrWhiteSpace(CustomErrorMessage) ? $": {CustomErrorMessage}" : null)}{(!Enabled ? " (Disabled)" : string.Empty)}";
}
