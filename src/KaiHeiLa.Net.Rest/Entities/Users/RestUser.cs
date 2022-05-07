using System.Diagnostics;
using System.Globalization;
using KaiHeiLa.API;
using KaiHeiLa.WebSocket;
using Model = KaiHeiLa.API.User;

namespace KaiHeiLa.Rest;

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
    public bool? IsVIP { get; internal set; }
    /// <inheritdoc />
    public string Avatar { get; internal set; }
    /// <inheritdoc />
    public string VIPAvatar { get; internal set; }
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
    public bool? IsOnline => Presence.IsOnline;
    /// <inheritdoc />
    public ClientType? ActiveClient => Presence.ActiveClient;
    
    internal RestUser(BaseKaiHeiLaClient kaiHeiLa, ulong id)
        : base(kaiHeiLa, id)
    {
    }
    internal static RestUser Create(BaseKaiHeiLaClient kaiHeiLa, Model model)
    {
        RestUser entity = new RestUser(kaiHeiLa, model.Id);
        entity.Update(model);
        return entity;
    }

    internal static RestUser Create(BaseKaiHeiLaClient kaiHeiLa, API.MentionUser model)
        => Create(kaiHeiLa, null, model);
    internal static RestUser Create(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, API.MentionUser model)
    {
        RestUser entity = new RestUser(kaiHeiLa, model.Id);
        entity.Update(model);
        return entity;
    }
    internal virtual void Update(Model model)
    {
        Username = model.Username;
        IdentifyNumberValue = ushort.Parse(model.IdentifyNumber, NumberStyles.None, CultureInfo.InvariantCulture);
        IsBot = model.Bot;
        IsBanned = model.Status == 10;
        IsVIP = model.IsVIP;
        Avatar = model.Avatar;
        VIPAvatar = model.VIPAvatar;
        IsDenoiseEnabled = model.IsDenoiseEnabled;
        UserTag = model.UserTag?.ToEntity();
        
        UpdatePresence(model.Online, model.OperatingSystem);
    }

    internal virtual void Update(API.MentionUser model)
    {
        Username = model.Username;
        IdentifyNumberValue = model.FullName.Length > 4 && ushort.TryParse(model.FullName[^4..], out ushort val) ? val : null;
        Avatar = model.Avatar;
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(RequestOptions options = null)
    {
        var model = await KaiHeiLa.ApiClient.GetUserAsync(Id, options).ConfigureAwait(false);
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
        => UserHelper.CreateDMChannelAsync(this, KaiHeiLa, options);

    /// <summary>
    ///     Gets the intimacy information with this user.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for getting the intimacy information. The task result
    ///     contains the intimacy information associated with this user.
    /// </returns>
    public Task<RestIntimacy> GetIntimacyAsync(RequestOptions options = null)
        => UserHelper.GetIntimacyAsync(this, KaiHeiLa, options);

    /// <inheritdoc />
    public async Task UpdateIntimacyAsync(Action<IntimacyProperties> func, RequestOptions options = null)
    {
        await UserHelper.UpdateIntimacyAsync(this, KaiHeiLa, func, options).ConfigureAwait(false);
    }
    #endregion
    
    /// <summary>
    ///     Gets the Username#IdentifyNumber of the user.
    /// </summary>
    /// <returns>
    ///     A string that resolves to Username#IdentifyNumber of the user.
    /// </returns>
    public override string ToString() => Format.UsernameAndIdentifyNumber(this);

    private string DebuggerDisplay => $"{Format.UsernameAndIdentifyNumber(this)} ({Id}{(IsBot ?? false ? ", Bot" : "")})";
    
    
    #region IUser
    
    /// <inheritdoc />
    async Task<IDMChannel> IUser.CreateDMChannelAsync(RequestOptions options)
        => await CreateDMChannelAsync(options).ConfigureAwait(false);
    /// <inheritdoc />
    async Task<IIntimacy> IUser.GetIntimacyAsync(RequestOptions options)
        => await GetIntimacyAsync(options).ConfigureAwait(false);
    
    #endregion
}