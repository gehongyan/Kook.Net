using System.Diagnostics;
using Model = Kook.API.User;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based user.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestUser : RestEntity<ulong>, IUser, IUpdateable
{
    private const int BannedStatus = 10;

    #region RestUser

    /// <inheritdoc />
    public string Username { get; internal set; }

    /// <inheritdoc />
    public ushort IdentifyNumberValue { get; internal set; }

    /// <inheritdoc />
    public bool? IsBot { get; internal set; }

    /// <inheritdoc />
    public bool? IsBanned { get; internal set; }

    /// <inheritdoc />
    public bool? HasBuff { get; internal set; }

    /// <inheritdoc />
    public bool? HasAnnualBuff { get; internal set; }

    /// <inheritdoc />
    public string Avatar { get; internal set; }

    /// <inheritdoc />
    public string? BuffAvatar { get; internal set; }

    /// <inheritdoc />
    public string? Banner { get; internal set; }

    /// <inheritdoc />
    public bool? IsDenoiseEnabled { get; internal set; }

    /// <inheritdoc />
    public UserTag? UserTag { get; internal set; }

    /// <inheritdoc />
    public IReadOnlyCollection<Nameplate> Nameplates { get; internal set; }

    /// <inheritdoc />
    public bool IsSystemUser { get; internal set; }

    /// <inheritdoc />
    public string IdentifyNumber => IdentifyNumberValue.ToString("D4");

    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionUser(Id);

    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionUser(Username, Id);

    internal RestPresence Presence { get; set; }

    /// <inheritdoc />
    public bool? IsOnline => Presence.IsOnline;

    /// <inheritdoc />
    public ClientType? ActiveClient => Presence?.ActiveClient;

    internal RestUser(BaseKookClient kook, ulong id)
        : base(kook, id)
    {
        Username = string.Empty;
        Avatar = string.Empty;
        Banner = string.Empty;
        Nameplates = [];
        Presence = new RestPresence();
        IsSystemUser = Id == KookConfig.SystemMessageAuthorID;
    }

    internal static RestUser Create(BaseKookClient kook, Model model)
    {
        RestUser entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal static RestUser Create(BaseKookClient kook, API.MentionedUser model)
    {
        RestUser entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal virtual void Update(Model model)
    {
        Username = model.Username;
        IdentifyNumberValue = ushort.Parse(model.IdentifyNumber);
        IsBot = model.Bot;
        IsBanned = model.Status == BannedStatus;
        HasBuff = model.HasBuff;
        HasAnnualBuff = model.HasAnnualBuff;
        Avatar = model.Avatar;
        Banner = model.Banner;
        BuffAvatar = model.BuffAvatar;
        IsDenoiseEnabled = model.IsDenoiseEnabled;
        UserTag = model.UserTag?.ToEntity();
        if (model.Nameplates is not null)
            Nameplates = [..model.Nameplates.Select(x => x.ToEntity())];
        if (model.IsSystemUser.HasValue)
            IsSystemUser = model.IsSystemUser.Value;
        UpdatePresence(model.Online, model.OperatingSystem);
    }

    internal virtual void Update(API.MentionedUser model)
    {
        Username = model.DisplayName;
        if (model.FullName.Length >= 5)
        {
            IdentifyNumberValue = ushort.Parse(model.FullName[^4..]);
            Username = model.FullName[..^5];
        }
        else
            Username = model.DisplayName;
        Avatar = model.Avatar;
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(RequestOptions? options = null)
    {
        Model model = await Kook.ApiClient.GetUserAsync(Id, options).ConfigureAwait(false);
        Update(model);
    }

    internal virtual void UpdatePresence(bool? isOnline, string? activeClient)
    {
        Presence.Update(isOnline, activeClient);
    }

    /// <summary>
    ///     Creates a direct message channel to this user.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task that represents the asynchronous get operation. The task result contains a rest DM channel where the user is the recipient. </returns>
    public Task<RestDMChannel> CreateDMChannelAsync(RequestOptions? options = null) =>
        UserHelper.CreateDMChannelAsync(this, Kook, options);

    /// <summary>
    ///     Gets the intimacy information with this user.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous operation for getting the intimacy information. The task result
    ///     contains the intimacy information associated with this user.
    /// </returns>
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
    public virtual Task RequestFriendAsync(RequestOptions? options = null) =>
        UserHelper.RequestFriendAsync(this, Kook, options);

    /// <inheritdoc />
    public Task RemoveFriendAsync(RequestOptions? options = null) =>
        UserHelper.RemoveFriendAsync(this, Kook, options);

    #endregion

    /// <summary>
    ///     获取此用户的包含用户名及识别号的格式化字符串。
    /// </summary>
    /// <returns> 一个表示此用户的格式化字符串。 </returns>
    /// <seealso cref="M:Kook.Format.UsernameAndIdentifyNumber(Kook.IUser,System.Boolean)"/>
    public override string ToString() => this.UsernameAndIdentifyNumber(Kook.FormatUsersInBidirectionalUnicode);

    private string DebuggerDisplay =>
        $"{this.UsernameAndIdentifyNumber(Kook.FormatUsersInBidirectionalUnicode)} ({Id}{
            (IsBot ?? false ? ", Bot" : "")})";

    #region IUser

    /// <inheritdoc />
    async Task<IDMChannel> IUser.CreateDMChannelAsync(RequestOptions? options) =>
        await CreateDMChannelAsync(options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IIntimacy> IUser.GetIntimacyAsync(RequestOptions? options) =>
        await GetIntimacyAsync(options).ConfigureAwait(false);

    #endregion
}
