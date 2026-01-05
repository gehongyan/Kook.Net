using System.Diagnostics;
using Model = Kook.API.User;

namespace Kook.WebSocket;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal sealed class SocketGlobalUser : SocketUser
{
#if NET9_0_OR_GREATER
    private readonly Lock _lockObj = new();
#else
    private readonly object _lockObj = new();
#endif
    private ushort _references;

    /// <inheritdoc />
    public override string Username { get; internal set; }

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
    public override string Avatar { get; internal set; }

    /// <inheritdoc />
    public override string? BuffAvatar { get; internal set; }

    /// <inheritdoc />
    public override string? Banner { get; internal set; }

    /// <inheritdoc />
    public override bool? IsDenoiseEnabled { get; internal set; }

    /// <inheritdoc />
    public override UserTag? UserTag { get; internal set; }

    /// <inheritdoc />
    public override IReadOnlyCollection<Nameplate> Nameplates { get; internal set; }

    /// <inheritdoc />
    internal override SocketPresence Presence { get; set; }

    /// <inheritdoc />
    internal override SocketGlobalUser GlobalUser => this;

    public SocketGlobalUser(KookSocketClient kook, ulong id)
        : base(kook, id)
    {
        Username = string.Empty;
        Avatar = string.Empty;
        Banner = string.Empty;
        Nameplates = [];
        Presence = new SocketPresence();
    }

    internal static SocketGlobalUser Create(KookSocketClient kook, ClientState state, Model model)
    {
        SocketGlobalUser entity = new(kook, model.Id);
        entity.Update(state, model);
        entity.UpdatePresence(model.Online, model.OperatingSystem);
        return entity;
    }

    internal void AddRef()
    {
        checked
        {
            lock (_lockObj)
            {
                _references++;
            }
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

    private string DebuggerDisplay =>
        $"{this.UsernameAndIdentifyNumber(Kook.FormatUsersInBidirectionalUnicode)} ({Id}{
            (IsBot ?? false ? ", Bot" : "")}, Global)";
    internal new SocketGlobalUser Clone() => (SocketGlobalUser)MemberwiseClone();
}
