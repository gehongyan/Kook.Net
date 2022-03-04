using System.Diagnostics;
using System.Globalization;
using KaiHeiLa.API;
using Model = KaiHeiLa.API.User;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestUser : RestEntity<ulong>, IUser, IUpdateable
{
    #region RestUser

    public string Username { get; internal set; }
    public ushort? IdentifyNumberValue { get; internal set; }
    public bool? IsOnline { get; internal set; }
    public bool? IsBot { get; internal set; }
    public bool? IsBanned { get; internal set; }
    public bool? IsVIP { get; internal set; }
    public string Avatar { get; internal set; }
    public string VIPAvatar { get; internal set; }

    /// <inheritdoc />
    public string IdentifyNumber => IdentifyNumberValue?.ToString("D4");
    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionUser(Id);
    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionUser(Username, Id);

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
        IsOnline = model.Online;
        IsBot = model.Bot;
        IsBanned = model.Status == 10;
        IsVIP = model.IsVIP;
        Avatar = model.Avatar;
        VIPAvatar = model.VIPAvatar;
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
    #endregion
    
    /// <summary>
    ///     Gets the Username#IdentifyNumber of the user.
    /// </summary>
    /// <returns>
    ///     A string that resolves to Username#IdentifyNumber of the user.
    /// </returns>
    public override string ToString() => Format.UsernameAndIdentifyNumber(this);

    private string DebuggerDisplay => $"{Format.UsernameAndIdentifyNumber(this)} ({Id}{(IsBot ?? false ? ", Bot" : "")})";
}