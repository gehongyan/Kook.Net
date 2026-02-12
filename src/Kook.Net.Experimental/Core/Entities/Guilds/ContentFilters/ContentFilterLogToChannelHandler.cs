using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个内容过滤器日志记录到频道的处理器。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct ContentFilterLogToChannelHandler : IContentFilterHandler
{
    /// <summary>
    ///     获取处理器的类型。
    /// </summary>
    public ContentFilterHandlerType Type => ContentFilterHandlerType.LogToChannel;

    /// <inheritdoc />
    public bool Enabled { get; init; }

    /// <inheritdoc />
    public string? Name { get; init; }

    /// <summary>
    ///     获取日志频道的标识。
    /// </summary>
    public ulong ChannelId { get; init; }

    private string DebuggerDisplay => $"LogToChannel: {Name} ({ChannelId}{(!Enabled ? ", Disabled" : string.Empty)})";
}
