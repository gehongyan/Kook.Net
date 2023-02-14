using System.Diagnostics;
using Model = Kook.API.User;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based user that is yet to be recognized by the client.
/// </summary>
/// <remarks>
///     A user may not be recognized due to the user missing from the cache or failed to be recognized properly.
/// </remarks>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketUnknownUser : SocketUser
{
    /// <inheritdoc />
    public override string Username { get; internal set; }
    /// <inheritdoc />
    public override ushort? IdentifyNumberValue { get; internal set; }
    /// <inheritdoc />
    public override bool? IsBot { get; internal set; }
    /// <inheritdoc />
    public override bool? IsBanned { get; internal set; }
    /// <inheritdoc />
    public override bool? HasBuff { get; internal set; }
    /// <inheritdoc />
    public override string Avatar { get; internal set; }
    /// <inheritdoc />
    public override string BuffAvatar { get; internal set; }
    /// <inheritdoc />
    public override string Banner { get; internal set; }
    /// <inheritdoc />
    public override bool? IsDenoiseEnabled { get; internal set; }
    /// <inheritdoc />
    public override UserTag UserTag { get; internal set; }
    /// <inheritdoc />
    internal override SocketPresence Presence { get => new SocketPresence(null, null); set { } }
    /// <inheritdoc />
    /// <exception cref="NotSupportedException">This field is not supported for an unknown user.</exception>
    internal override SocketGlobalUser GlobalUser => throw new NotSupportedException();

    internal SocketUnknownUser(KookSocketClient kook, ulong id)
        : base(kook, id)
    {
    }

    internal static SocketUnknownUser Create(KookSocketClient kook, ClientState state, ulong id)
    {
        var entity = new SocketUnknownUser(kook, id);
        return entity;
    }

    private string DebuggerDisplay => $"{Username}#{IdentifyNumber} ({Id}{(IsBot ?? false ? ", Bot" : "")}, Unknown)";
    internal new SocketUnknownUser Clone() => MemberwiseClone() as SocketUnknownUser;
}
