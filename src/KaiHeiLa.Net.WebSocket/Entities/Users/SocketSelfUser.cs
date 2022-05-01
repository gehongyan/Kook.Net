using Model = KaiHeiLa.API.Rest.SelfUser;

using System.Diagnostics;
using System.Globalization;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents the logged-in WebSocket-based user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketSelfUser : SocketUser, ISelfUser
{
    internal override SocketGlobalUser GlobalUser { get; }
    
    /// <inheritdoc />
    public override bool? IsBot { get => GlobalUser.IsBot; internal set => GlobalUser.IsBot = value; }
    /// <inheritdoc />
    public override string Username { get => GlobalUser.Username; internal set => GlobalUser.Username = value; }
    /// <inheritdoc />
    public override ushort? IdentifyNumberValue { get => GlobalUser.IdentifyNumberValue; internal set => GlobalUser.IdentifyNumberValue = value; }
    /// <inheritdoc />
    public override string Avatar { get => GlobalUser.Avatar; internal set => GlobalUser.Avatar = value; }
    /// <inheritdoc />
    public override string VIPAvatar { get => GlobalUser.VIPAvatar; internal set => GlobalUser.VIPAvatar = value; }
    /// <inheritdoc />
    public override bool? IsBanned { get => GlobalUser.IsBanned; internal set => GlobalUser.IsBanned = value; }
    /// <inheritdoc />
    public override bool? IsVIP { get => GlobalUser.IsVIP; internal set => GlobalUser.IsVIP = value; }
    /// <inheritdoc />
    public override bool? IsDenoiseEnabled { get => GlobalUser.IsDenoiseEnabled; internal set => GlobalUser.IsDenoiseEnabled = value; }
    /// <inheritdoc />
    public override UserTag UserTag { get => GlobalUser.UserTag; internal set => GlobalUser.UserTag = value; }
    /// <inheritdoc />
    internal override SocketPresence Presence { get => GlobalUser.Presence; set => GlobalUser.Presence = value;
    }
    /// <inheritdoc />
    public string MobilePrefix { get; internal set; }
    /// <inheritdoc />
    public string Mobile { get; internal set; }
    /// <inheritdoc />
    public int InvitedCount { get; internal set; }
    /// <inheritdoc />
    public bool IsMobileVerified { get; internal set; }

    internal SocketSelfUser(KaiHeiLaSocketClient kaiHeiLa, SocketGlobalUser globalUser)
        : base(kaiHeiLa, globalUser.Id)
    {
        GlobalUser = globalUser;
    }
    internal static SocketSelfUser Create(KaiHeiLaSocketClient kaiHeiLa, ClientState state, Model model)
    {
        var entity = new SocketSelfUser(kaiHeiLa, kaiHeiLa.GetOrCreateSelfUser(state, model));
        entity.Update(state, model);
        entity.UpdatePresence(model.Online, model.OperatingSystem);
        return entity;
    }

    internal bool Update(ClientState state, Model model)
    {
        bool hasGlobalChanges = base.Update(state, model);
        UpdatePresence(model.Online, model.OperatingSystem);
        if (model.MobilePrefix != MobilePrefix)
        {
            MobilePrefix = model.MobilePrefix;
            hasGlobalChanges = true;
        }
        if (model.Mobile != Mobile)
        {
            Mobile = model.Mobile;
            hasGlobalChanges = true;
        }
        if (model.InvitedCount != InvitedCount)
        {
            InvitedCount = model.InvitedCount ?? 0;
            hasGlobalChanges = true;
        }
        if (model.MobileVerified != IsMobileVerified)
        {
            IsMobileVerified = model.MobileVerified;
            hasGlobalChanges = true;
        }
        return hasGlobalChanges;
    }
    
    private string DebuggerDisplay => $"{Username}#{IdentifyNumber} ({Id}{(IsBot ?? false ? ", Bot" : "")}, Self)";
    internal new SocketSelfUser Clone() => MemberwiseClone() as SocketSelfUser;

    #region ISelfUser
    
    /// <inheritdoc />
    public async Task StartPlayingAsync(IGame game, RequestOptions options = null)
        => await UserHelper.StartPlayingAsync(this, KaiHeiLa, game, options).ConfigureAwait(false);
    /// <inheritdoc />
    public async Task StopPlayingAsync(RequestOptions options = null)
        => await UserHelper.StopPlayingAsync(this, KaiHeiLa, options).ConfigureAwait(false);

    #endregion
}