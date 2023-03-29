using System.Diagnostics;
using System.Globalization;
using Model = Kook.API.User;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestUser : RestEntity<ulong>, IUser, IUpdateable
{
    #region RestUser

    /// <inheritdoc />
    public string Username { get; internal set; }

    /// <inheritdoc />
    public ushort? IdentifyNumberValue { get; internal set; }

    /// <inheritdoc />
    public bool? IsBot { get; internal set; }

    /// <inheritdoc />
    public bool? IsBanned { get; internal set; }

    /// <inheritdoc />
    public bool? HasBuff { get; internal set; }

    /// <inheritdoc />
    public string Avatar { get; internal set; }

    /// <inheritdoc />
    public string BuffAvatar { get; internal set; }

    /// <inheritdoc />
    public string Banner { get; internal set; }

    /// <inheritdoc />
    public bool? IsDenoiseEnabled { get; internal set; }

    /// <inheritdoc />
    public UserTag UserTag { get; internal set; }

    /// <inheritdoc />
    public string IdentifyNumber => IdentifyNumberValue?.ToString("D4");

    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionUser(Id);

    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionUser(Username, Id);

    internal RestPresence Presence { get; set; }

    /// <inheritdoc />
    public bool? IsOnline => Presence?.IsOnline;

    /// <inheritdoc />
    public ClientType? ActiveClient => Presence?.ActiveClient;

    internal RestUser(BaseKookClient kook, ulong id)
        : base(kook, id)
    {
    }

    internal static RestUser Create(BaseKookClient kook, Model model)
    {
        RestUser entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal static RestUser Create(BaseKookClient kook, API.MentionedUser model)
        => Create(kook, null, model);

    internal static RestUser Create(BaseKookClient kook, IGuild guild, API.MentionedUser model)
    {
        RestUser entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal virtual void Update(Model model)
    {
        Username = model.Username;
        IdentifyNumberValue = ushort.Parse(model.IdentifyNumber, NumberStyles.None, CultureInfo.InvariantCulture);
        IsBot = model.Bot;
        IsBanned = model.Status == 10;
        HasBuff = model.HasBuff;
        Avatar = model.Avatar;
        Banner = model.Banner;
        BuffAvatar = model.BuffAvatar;
        IsDenoiseEnabled = model.IsDenoiseEnabled;
        UserTag = model.UserTag?.ToEntity();

        UpdatePresence(model.Online, model.OperatingSystem);
    }

    internal virtual void Update(API.MentionedUser model)
    {
        Username = model.Username;
        IdentifyNumberValue = model.FullName.Length > 4
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            && ushort.TryParse(model.FullName[^4..], out ushort val)
#else
            && ushort.TryParse(model.FullName.Substring(model.FullName.Length - 4), out ushort val)
#endif
                ? val
                : null;
        Avatar = model.Avatar;
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(RequestOptions options = null)
    {
        Model model = await Kook.ApiClient.GetUserAsync(Id, options).ConfigureAwait(false);
        Update(model);
    }

    internal virtual void UpdatePresence(bool? isOnline, string activeClient)
    {
        Presence ??= new RestPresence();
        Presence.Update(isOnline, activeClient);
    }

    /// <summary>
    ///     Creates a direct message channel to this user.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a rest DM channel where the user is the recipient.
    /// </returns>
    public Task<RestDMChannel> CreateDMChannelAsync(RequestOptions options = null)
        => UserHelper.CreateDMChannelAsync(this, Kook, options);

    /// <summary>
    ///     Gets the intimacy information with this user.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for getting the intimacy information. The task result
    ///     contains the intimacy information associated with this user.
    /// </returns>
    public Task<RestIntimacy> GetIntimacyAsync(RequestOptions options = null)
        => UserHelper.GetIntimacyAsync(this, Kook, options);

    /// <inheritdoc />
    public async Task UpdateIntimacyAsync(Action<IntimacyProperties> func, RequestOptions options = null) =>
        await UserHelper.UpdateIntimacyAsync(this, Kook, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    public Task BlockAsync(RequestOptions options = null) =>
        UserHelper.BlockAsync(this, Kook, options);

    /// <inheritdoc />
    public Task UnblockAsync(RequestOptions options = null) =>
        UserHelper.UnblockAsync(this, Kook, options);

    /// <inheritdoc />
    public virtual Task RequestFriendAsync(RequestOptions options = null) =>
        UserHelper.RequestFriendAsync(this, Kook, options);

    /// <inheritdoc />
    public Task RemoveFriendAsync(RequestOptions options = null) =>
        UserHelper.RemoveFriendAsync(this, Kook, options);

    #endregion

    /// <summary>
    ///     Gets the Username#IdentifyNumber of the user.
    /// </summary>
    /// <returns>
    ///     A string that resolves to Username#IdentifyNumber of the user.
    /// </returns>
    public override string ToString() => Format.UsernameAndIdentifyNumber(this, Kook.FormatUsersInBidirectionalUnicode);

    private string DebuggerDisplay =>
        $"{Format.UsernameAndIdentifyNumber(this, Kook.FormatUsersInBidirectionalUnicode)} ({Id}{(IsBot ?? false ? ", Bot" : "")})";

    #region IUser

    /// <inheritdoc />
    async Task<IDMChannel> IUser.CreateDMChannelAsync(RequestOptions options)
        => await CreateDMChannelAsync(options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IIntimacy> IUser.GetIntimacyAsync(RequestOptions options)
        => await GetIntimacyAsync(options).ConfigureAwait(false);

    #endregion
}
