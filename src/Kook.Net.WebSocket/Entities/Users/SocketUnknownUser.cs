using System.Diagnostics;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based user that is yet to be recognized by the client.
/// </summary>
/// <remarks>
///     A user may not be recognized due to the user missing from the cache or failed to be recognized properly.
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
    internal override SocketPresence Presence
    {
        get => new();
        set { }
    }

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">This field is not supported for an unknown user.</exception>
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
