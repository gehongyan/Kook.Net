using System.Diagnostics;
using Model = KaiHeiLa.API.User;

namespace KaiHeiLa.WebSocket;

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
    public override bool? IsVIP { get; internal set; }
    /// <inheritdoc />
    public override string Avatar { get; internal set; }
    /// <inheritdoc />
    public override string VIPAvatar { get; internal set; }
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
    
    public SocketGlobalUser(KaiHeiLaSocketClient kaiHeiLa, ulong id) 
        : base(kaiHeiLa, id)
    {
    }
    internal static SocketGlobalUser Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, Model model)
    {
        var entity = new SocketGlobalUser(kaiHeiLa, model.Id);
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
    internal void RemoveRef(KaiHeiLaSocketClient kaiHeiLa)
    {
        lock (_lockObj)
        {
            if (--_references <= 0)
                kaiHeiLa.RemoveUser(Id);
        }
    }
    
    private string DebuggerDisplay => $"{Username}#{IdentifyNumber} ({Id}{(IsBot ?? false ? ", Bot" : "")}, Global)";
    internal new SocketGlobalUser Clone() => MemberwiseClone() as SocketGlobalUser;
}