using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的未知用户。
/// </summary>
/// <remarks>
///     如果用户未能被识别，或缓存中不存在该用户，则会使用此类型的用户实体设置需要用户实体的属性。
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketUnknownUser : SocketUser
{
    /// <inheritdoc />
    public override string Username { get; internal set; } = string.Empty;

    /// <inheritdoc />
    public override ushort IdentifyNumberValue { get; internal set; }

    /// <inheritdoc />
    public override bool? IsBot { get; internal set; }

    /// <inheritdoc />
    public override bool? IsBanned { get; internal set; }

    /// <inheritdoc />
    public override bool? HasBuff { get; internal set; }

    /// <inheritdoc />
    public override bool? HasAnnualBuff { get; internal set; }

    /// <inheritdoc />
    public override string Avatar { get; internal set; } = string.Empty;

    /// <inheritdoc />
    public override string? BuffAvatar { get; internal set; } = string.Empty;

    /// <inheritdoc />
    public override string? Banner { get; internal set; } = string.Empty;

    /// <inheritdoc />
    public override bool? IsDenoiseEnabled { get; internal set; }

    /// <inheritdoc />
    public override UserTag? UserTag { get; internal set; }

    /// <inheritdoc />
    public override IReadOnlyCollection<Nameplate> Nameplates { get; internal set; } = [];

    /// <inheritdoc />
    public override KpmVipInfo? KpmVipInfo { get; internal set; }

    /// <inheritdoc />
    public override int VoiceWealthLevel { get; internal set; }

    internal override SocketPresence Presence
    {
        get => new();
        set { }
    }

    internal override SocketGlobalUser GlobalUser => throw new NotSupportedException();

    internal SocketUnknownUser(KookSocketClient kook, ulong id)
        : base(kook, id)
    {
    }

    internal static SocketUnknownUser Create(KookSocketClient kook, ClientState state, ulong id)
    {
        SocketUnknownUser entity = new(kook, id);
        return entity;
    }

    private string DebuggerDisplay =>
        $"{this.UsernameAndIdentifyNumber(Kook.FormatUsersInBidirectionalUnicode)} ({Id}{
            (IsBot ?? false ? ", Bot" : "")}, Unknown)";
    internal new SocketUnknownUser Clone() => (SocketUnknownUser)MemberwiseClone();
}
