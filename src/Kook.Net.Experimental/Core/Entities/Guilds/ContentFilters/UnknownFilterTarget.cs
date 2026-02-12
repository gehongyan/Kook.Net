using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个未知类型的内容过滤器的过滤目标。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct UnknownFilterTarget : IContentFilterTarget
{
    /// <inheritdoc />
    public ContentFilterType Type => 0;

    /// <inheritdoc />
    public ContentFilterMode Mode { get; init; }

    private string DebuggerDisplay => $"UnknownFilter: {Mode}";
}
