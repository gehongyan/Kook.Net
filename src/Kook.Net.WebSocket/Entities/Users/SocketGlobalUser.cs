using System.Diagnostics;
using Model = Kook.API.User;

namespace Kook.WebSocket;

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
internal class SocketGlobalUser : SocketUser
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
    internal override SocketPresence Presence { get; set; }
    /// <inheritdoc />
    internal override SocketGlobalUser GlobalUser => this;

    private readonly object _lockObj = new object();
    private ushort _references;

    public SocketGlobalUser(KookSocketClient kook, ulong id)
        : base(kook, id)
    {
    }
    internal static SocketGlobalUser Create(KookSocketClient kook, ClientState state, Model model)
    {
        var entity = new SocketGlobalUser(kook, model.Id);
        entity.Update(state, model);
        entity.UpdatePresence(model.Online, model.OperatingSystem);
        return entity;
    }

    internal void AddRef()
    {
        checked
        {
            lock (_lockObj)
                _references++;
        }
    }
    internal void RemoveRef(KookSocketClient kook)
    {
        lock (_lockObj)
        {
            if (--_references <= 0)
                kook.RemoveUser(Id);
        }
    }

    private string DebuggerDisplay => $"{Username}#{IdentifyNumber} ({Id}{(IsBot ?? false ? ", Bot" : "")}, Global)";
    internal new SocketGlobalUser Clone() => MemberwiseClone() as SocketGlobalUser;
}
