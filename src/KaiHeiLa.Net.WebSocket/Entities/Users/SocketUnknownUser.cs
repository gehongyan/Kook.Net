using System.Diagnostics;
using Model = KaiHeiLa.API.User;

namespace KaiHeiLa.WebSocket;

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
    public override bool? IsOnline { get; internal set; }
    public override bool? IsBot { get; internal set; }
    public override bool? IsBanned { get; internal set; }
    public override bool? IsVIP { get; internal set; }
    public override string Avatar { get; internal set; }
    public override string VIPAvatar { get; internal set; }

    /// <exception cref="NotSupportedException">This field is not supported for an unknown user.</exception>
    internal override SocketGlobalUser GlobalUser =>
        throw new NotSupportedException();

    internal SocketUnknownUser(KaiHeiLaSocketClient kaiHeiLa, ulong id)
        : base(kaiHeiLa, id)
    {
    }

    internal static SocketUnknownUser Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, ulong id)
    {
        var entity = new SocketUnknownUser(kaiHeiLa, id);
        return entity;
    }
    
    private string DebuggerDisplay => $"{Username}#{IdentifyNumber} ({Id}{(IsBot ?? false ? ", Bot" : "")}, Unknown)";
    internal new SocketUnknownUser Clone() => MemberwiseClone() as SocketUnknownUser;
}