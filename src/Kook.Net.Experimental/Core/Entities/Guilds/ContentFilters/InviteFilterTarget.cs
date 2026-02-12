using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个服务器邀请链接过滤器的过滤目标。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct InviteFilterTarget : IContentFilterTarget
{
    /// <inheritdoc />
    public ContentFilterType Type => ContentFilterType.Invite;

    /// <inheritdoc />
    public ContentFilterMode Mode { get; init; }

    /// <summary>
    ///     获取被过滤的服务器 ID 列表。
    /// </summary>
    /// <remarks>
    ///     排除列表之外的服务器用户，根据 <see cref="Mode"/> 的模式，无法发送或只能发送包含其 ID 位于此列表中的服务器的邀请链接。
    /// </remarks>
    public IReadOnlyCollection<ulong> GuildIds { get; init; }

    private string DebuggerDisplay => $"InviteFilter: {Mode}{string.Concat(GuildIds.Select(x => $", {x}"))}";
}
