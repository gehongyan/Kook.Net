using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个词汇过滤器的过滤目标。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct WordFilterTarget : IContentFilterTarget
{
    /// <inheritdoc />
    public ContentFilterType Type => ContentFilterType.Word;

    /// <inheritdoc />
    public ContentFilterMode Mode => ContentFilterMode.Blacklist;

    /// <summary>
    ///     获取被过滤的词汇列表。
    /// </summary>
    /// <remarks>
    ///     排除列表之外的服务器用户无法发送包含这些词汇的消息。
    /// </remarks>
    public required IReadOnlyCollection<string> Words { get; init; }

    private string DebuggerDisplay => $"WordFilter: {string.Join(", ", Words)}";
}