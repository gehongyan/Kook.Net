using Kook.Rest;
using System.Diagnostics;
using System.Globalization;
using Model = Kook.API.User;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public abstract class SocketUser : SocketEntity<ulong>, IUser
{
    /// <inheritdoc />
    public abstract string Username { get; internal set; }
    /// <inheritdoc />
    public abstract ushort? IdentifyNumberValue { get; internal set; }
    /// <inheritdoc />
    public abstract bool? IsBot { get; internal set; }
    /// <inheritdoc />
    public abstract bool? IsBanned { get; internal set; }
    /// <inheritdoc />
    public abstract bool? HasBuff { get; internal set; }
    /// <inheritdoc />
    public abstract string Avatar { get; internal set; }
    /// <inheritdoc />
    public abstract string BuffAvatar { get; internal set; }
    /// <inheritdoc />
    public abstract string Banner { get; internal set; }
    /// <inheritdoc />
    public abstract bool? IsDenoiseEnabled { get; internal set; }
    /// <inheritdoc />
    public abstract UserTag UserTag { get; internal set; }
    internal abstract SocketGlobalUser GlobalUser { get; }
    internal abstract SocketPresence Presence { get; set; }

    /// <inheritdoc />
    public string IdentifyNumber => IdentifyNumberValue?.ToString("D4");
    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionUser(Id);
    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionUser(Username, Id);

    /// <inheritdoc />
    public bool? IsOnline => Presence.IsOnline;
    /// <inheritdoc />
    public ClientType? ActiveClient => Presence.ActiveClient;

    protected SocketUser(KookSocketClient kook, ulong id)
        : base(kook, id)
    {
    }

    internal virtual bool Update(ClientState state, API.Gateway.UserUpdateEvent model)
    {
        bool hasChanges = false;

        if (model.Username != Username)
        {
            Username = model.Username;
            hasChanges = true;
        }

        if (model.Avatar != Avatar)
        {
            Avatar = model.Avatar;
            hasChanges = true;
        }

        return hasChanges;
    }
    internal virtual bool Update(ClientState state, Model model)
    {
        bool hasChanges = false;

        if (model.Username != Username)
        {
            Username = model.Username;
            hasChanges = true;
        }
        if (model.IdentifyNumber != IdentifyNumber)
        {
            ushort newVal = ushort.Parse(model.IdentifyNumber, NumberStyles.None, CultureInfo.InvariantCulture);
            if (newVal != IdentifyNumberValue)
            {
                IdentifyNumberValue = newVal;
                hasChanges = true;
            }
        }
        if (model.Bot != IsBot)
        {
            IsBot = model.Bot;
            hasChanges = true;
        }
        if (model.Status == 10 != IsBanned)
        {
            IsBanned = model.Status == 10;
            hasChanges = true;
        }
        if (model.HasBuff != HasBuff)
        {
            HasBuff = model.HasBuff;
            hasChanges = true;
        }
        if (model.Avatar != Avatar)
        {
            Avatar = model.Avatar;
            hasChanges = true;
        }
        if (model.BuffAvatar != BuffAvatar)
        {
            BuffAvatar = model.BuffAvatar;
            hasChanges = true;
        }
        if (model.Banner != Banner)
        {
            Banner = model.Banner;
            hasChanges = true;
        }
        if (model.IsDenoiseEnabled != IsDenoiseEnabled)
        {
            IsDenoiseEnabled = model.IsDenoiseEnabled;
            hasChanges = true;
        }
        if (model.UserTag is not null && !model.UserTag.ToEntity().Equals(UserTag))
        {
            UserTag = model.UserTag.ToEntity();
            hasChanges = true;
        }

        return hasChanges;
    }

    internal virtual void UpdatePresence(bool? isOnline)
    {
        Presence ??= new SocketPresence();
        Presence.Update(isOnline);
    }
    internal virtual void UpdatePresence(bool? isOnline, string activeClient)
    {
        Presence ??= new SocketPresence();
        Presence.Update(isOnline, activeClient);
    }

    /// <inheritdoc cref="IUser.CreateDMChannelAsync(RequestOptions)" />
    public async Task<SocketDMChannel> CreateDMChannelAsync(RequestOptions options = null)
        => await SocketUserHelper.CreateDMChannelAsync(this, Kook, options).ConfigureAwait(false);

    /// <inheritdoc cref="IUser.GetIntimacyAsync(RequestOptions)" />
    public Task<RestIntimacy> GetIntimacyAsync(RequestOptions options = null)
        => UserHelper.GetIntimacyAsync(this, Kook, options);

    /// <inheritdoc />
    public async Task UpdateIntimacyAsync(Action<IntimacyProperties> func, RequestOptions options = null)
        => await UserHelper.UpdateIntimacyAsync(this, Kook, func, options).ConfigureAwait(false);

    /// <summary>
    ///     Gets the full name of the user (e.g. Example#0001).
    /// </summary>
    /// <returns>
    ///     The full name of the user.
    /// </returns>
    public override string ToString() => Format.UsernameAndIdentifyNumber(this);
    private string DebuggerDisplay => $"{Format.UsernameAndIdentifyNumber(this)} ({Id}{(IsBot ?? false ? ", Bot" : "")})";
    internal SocketUser Clone() => MemberwiseClone() as SocketUser;

    #region IUser

    /// <inheritdoc />
    async Task<IDMChannel> IUser.CreateDMChannelAsync(RequestOptions options)
        => await CreateDMChannelAsync(options).ConfigureAwait(false);
    /// <inheritdoc />
    async Task<IIntimacy> IUser.GetIntimacyAsync(RequestOptions options)
        => await GetIntimacyAsync(options).ConfigureAwait(false);

    #endregion
}
