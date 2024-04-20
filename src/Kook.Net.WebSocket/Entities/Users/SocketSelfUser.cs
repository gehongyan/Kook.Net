using Kook.Rest;
using System.Diagnostics;
using Model = Kook.API.Rest.SelfUser;

namespace Kook.WebSocket;

/// <summary>
///     Represents the logged-in WebSocket-based user.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketSelfUser : SocketUser, ISelfUser
{
    internal override SocketGlobalUser GlobalUser { get; }

    /// <inheritdoc />
    public override bool? IsBot
    {
        get => GlobalUser.IsBot;
        internal set => GlobalUser.IsBot = value;
    }

    /// <inheritdoc />
    public override string Username
    {
        get => GlobalUser.Username;
        internal set => GlobalUser.Username = value;
    }

    /// <inheritdoc />
    public override ushort? IdentifyNumberValue
    {
        get => GlobalUser.IdentifyNumberValue;
        internal set => GlobalUser.IdentifyNumberValue = value;
    }

    /// <inheritdoc />
    public override string Avatar
    {
        get => GlobalUser.Avatar;
        internal set => GlobalUser.Avatar = value;
    }

    /// <inheritdoc />
    public override string BuffAvatar
    {
        get => GlobalUser.BuffAvatar;
        internal set => GlobalUser.BuffAvatar = value;
    }

    /// <inheritdoc />
    public override string Banner
    {
        get => GlobalUser.Banner;
        internal set => GlobalUser.Banner = value;
    }

    /// <inheritdoc />
    public override bool? IsBanned
    {
        get => GlobalUser.IsBanned;
        internal set => GlobalUser.IsBanned = value;
    }

    /// <inheritdoc />
    public override bool? HasBuff
    {
        get => GlobalUser.HasBuff;
        internal set => GlobalUser.HasBuff = value;
    }

    /// <inheritdoc />
    public override bool? HasAnnualBuff
    {
        get => GlobalUser.HasAnnualBuff;
        internal set => GlobalUser.HasAnnualBuff = value;
    }

    /// <inheritdoc />
    public override bool? IsDenoiseEnabled
    {
        get => GlobalUser.IsDenoiseEnabled;
        internal set => GlobalUser.IsDenoiseEnabled = value;
    }

    /// <inheritdoc />
    public override UserTag UserTag
    {
        get => GlobalUser.UserTag;
        internal set => GlobalUser.UserTag = value;
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<Nameplate> Nameplates
    {
        get => GlobalUser.Nameplates;
        internal set => GlobalUser.Nameplates = value;
    }

    /// <inheritdoc />
    public override bool? IsSystemUser
    {
        get => GlobalUser.IsSystemUser;
        internal set => GlobalUser.IsSystemUser = value;
    }

    /// <inheritdoc />
    internal override SocketPresence Presence
    {
        get => GlobalUser.Presence;
        set => GlobalUser.Presence = value;
    }

    /// <inheritdoc />
    public string MobilePrefix { get; internal set; }

    /// <inheritdoc />
    public string Mobile { get; internal set; }

    /// <inheritdoc />
    public int InvitedCount { get; internal set; }

    /// <inheritdoc />
    public bool IsMobileVerified { get; internal set; }

    internal SocketSelfUser(KookSocketClient kook, SocketGlobalUser globalUser)
        : base(kook, globalUser.Id) =>
        GlobalUser = globalUser;

    internal static SocketSelfUser Create(KookSocketClient kook, ClientState state, Model model)
    {
        SocketSelfUser entity = new(kook, kook.GetOrCreateSelfUser(state, model));
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
    public async Task StartPlayingAsync(IGame game, RequestOptions? options = null)
        => await UserHelper.StartPlayingAsync(this, Kook, game, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task StartPlayingAsync(Music music, RequestOptions? options = null)
        => await UserHelper.StartPlayingAsync(this, Kook, music, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task StopPlayingAsync(ActivityType type, RequestOptions? options = null)
        => await UserHelper.StopPlayingAsync(this, Kook, type, options).ConfigureAwait(false);

    #endregion
}
