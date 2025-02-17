using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using Kook.Rest;
using Model = Kook.API.User;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的用户。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public abstract class SocketUser : SocketEntity<ulong>, IUser
{
    /// <inheritdoc />
    public abstract string Username { get; internal set; }

    /// <inheritdoc />
    public abstract ushort IdentifyNumberValue { get; internal set; }

    /// <inheritdoc />
    public abstract bool? IsBot { get; internal set; }

    /// <inheritdoc />
    public abstract bool? IsBanned { get; internal set; }

    /// <inheritdoc />
    public abstract bool? HasBuff { get; internal set; }

    /// <inheritdoc />
    public abstract bool? HasAnnualBuff { get; internal set; }

    /// <inheritdoc />
    public abstract string Avatar { get; internal set; }

    /// <inheritdoc />
    public abstract string? BuffAvatar { get; internal set; }

    /// <inheritdoc />
    public abstract string? Banner { get; internal set; }

    /// <inheritdoc />
    public abstract bool? IsDenoiseEnabled { get; internal set; }

    /// <inheritdoc />
    public abstract UserTag? UserTag { get; internal set; }

    /// <inheritdoc />
    public abstract IReadOnlyCollection<Nameplate> Nameplates { get; internal set; }

    /// <inheritdoc />
    public bool IsSystemUser { get; internal set; }

    internal abstract SocketGlobalUser GlobalUser { get; }

    internal abstract SocketPresence Presence { get; set; }

    /// <inheritdoc />
    public string IdentifyNumber => IdentifyNumberValue.ToString("D4");

    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionUser(Id);

    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionUser(Username, Id);

    /// <inheritdoc />
    public bool? IsOnline => Presence?.IsOnline;

    /// <inheritdoc />
    public ClientType? ActiveClient => Presence?.ActiveClient;

    internal SocketUser(KookSocketClient kook, ulong id)
        : base(kook, id)
    {
        IsSystemUser = Id == KookConfig.SystemMessageAuthorID;
    }

    internal virtual bool Update(ClientState state, API.Gateway.UserUpdateEvent model)
    {
        bool hasChanges = false;
        hasChanges |= ValueHelper.SetIfChanged(() => Username, x => Username = x, model.Username);
        if (hasChanges)
        {
            foreach (SocketGuildUser current in state.Guilds.Select(x => x.GetUser(Id)).OfType<SocketGuildUser>())
                current.UpdateNickname();
        }

        hasChanges |= ValueHelper.SetIfChanged(() => Avatar, x => Avatar = x, model.Avatar);
        return hasChanges;
    }

    internal virtual bool Update(ClientState state, Model model)
    {
        bool hasChanges = false;
        hasChanges |= ValueHelper.SetIfChanged(() => Username, x => Username = x, model.Username);
        hasChanges |= ValueHelper.SetIfChanged(() => IdentifyNumberValue, x => IdentifyNumberValue = x,
            ushort.Parse(model.IdentifyNumber, NumberStyles.None, CultureInfo.InvariantCulture));
        hasChanges |= ValueHelper.SetIfChanged(() => IsBot, x => IsBot = x, model.Bot);
        hasChanges |= ValueHelper.SetIfChanged(() => IsBanned, x => IsBanned = x, model.Status == 10);
        hasChanges |= ValueHelper.SetIfChanged(() => HasBuff, x => HasBuff = x, model.HasBuff);
        hasChanges |= ValueHelper.SetIfChanged(() => HasAnnualBuff, x => HasAnnualBuff = x, model.HasAnnualBuff);
        hasChanges |= ValueHelper.SetIfChanged(() => Avatar, x => Avatar = x, model.Avatar);
        hasChanges |= ValueHelper.SetIfChanged(() => BuffAvatar, x => BuffAvatar = x, model.BuffAvatar);
        hasChanges |= ValueHelper.SetIfChanged(() => Banner, x => Banner = x, model.Banner);
        hasChanges |= ValueHelper.SetIfChanged(() => IsDenoiseEnabled, x => IsDenoiseEnabled = x, model.IsDenoiseEnabled);
        hasChanges |= ValueHelper.SetIfChanged(() => UserTag, x => UserTag = x, model.UserTag?.ToEntity());
        hasChanges |= ValueHelper.SetIfChanged(() => Nameplates, x => Nameplates = x,
            model.Nameplates?.Select(x => x.ToEntity()).ToImmutableList() ?? [],
            (x, y) => x.SequenceEqual(y));
        if (model.IsSystemUser.HasValue)
            hasChanges |= ValueHelper.SetIfChanged(() => IsSystemUser, x => IsSystemUser = x, model.IsSystemUser.Value);
        return hasChanges;
    }

    internal virtual void UpdatePresence(bool? isOnline)
    {
        Presence.Update(isOnline);
    }

    internal virtual void UpdatePresence(bool? isOnline, string? activeClient)
    {
        Presence.Update(isOnline, activeClient);
    }

    /// <inheritdoc cref="Kook.IUser.CreateDMChannelAsync(Kook.RequestOptions)" />
    public async Task<SocketDMChannel> CreateDMChannelAsync(RequestOptions? options = null) =>
        await SocketUserHelper.CreateDMChannelAsync(this, Kook, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IUser.GetIntimacyAsync(Kook.RequestOptions)" />
    public Task<RestIntimacy> GetIntimacyAsync(RequestOptions? options = null) =>
        UserHelper.GetIntimacyAsync(this, Kook, options);

    /// <inheritdoc />
    public async Task UpdateIntimacyAsync(Action<IntimacyProperties> func, RequestOptions? options = null) =>
        await UserHelper.UpdateIntimacyAsync(this, Kook, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    public Task BlockAsync(RequestOptions? options = null) =>
        UserHelper.BlockAsync(this, Kook, options);

    /// <inheritdoc />
    public Task UnblockAsync(RequestOptions? options = null) =>
        UserHelper.UnblockAsync(this, Kook, options);

    /// <inheritdoc />
    public Task RequestFriendAsync(RequestOptions? options = null) =>
        UserHelper.RequestFriendAsync(this, Kook, options);

    /// <inheritdoc />
    public Task RemoveFriendAsync(RequestOptions? options = null) =>
        UserHelper.RemoveFriendAsync(this, Kook, options);

    /// <inheritdoc />
    public Task RequestIntimacyRelationAsync(IntimacyRelationType relationType, RequestOptions? options = null) =>
        UserHelper.RequestIntimacyRelationAsync(this, relationType, Kook, options);

    /// <inheritdoc />
    public Task UnravelIntimacyRelationAsync(bool removeFriend = false, RequestOptions? options = null) =>
        UserHelper.UnravelIntimacyRelationAsync(this, removeFriend, Kook, options);

    /// <summary>
    ///     获取此用户的包含用户名及识别号的格式化字符串。
    /// </summary>
    /// <returns> 一个表示此用户的包含用户名及识别号的格式化字符串。 </returns>
    /// <seealso cref="Kook.Format.UsernameAndIdentifyNumber(Kook.IUser,System.Boolean)"/>
    public override string ToString() => Format.UsernameAndIdentifyNumber(this, Kook.FormatUsersInBidirectionalUnicode);

    private string DebuggerDisplay =>
        $"{Format.UsernameAndIdentifyNumber(this, Kook.FormatUsersInBidirectionalUnicode)} ({Id}{
            (IsBot ?? false ? ", Bot" : "")})";

    internal SocketUser Clone() => (SocketUser)MemberwiseClone();

    #region IUser

    /// <inheritdoc />
    async Task<IDMChannel> IUser.CreateDMChannelAsync(RequestOptions? options) =>
        await CreateDMChannelAsync(options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IIntimacy> IUser.GetIntimacyAsync(RequestOptions? options) =>
        await GetIntimacyAsync(options).ConfigureAwait(false);

    #endregion
}
