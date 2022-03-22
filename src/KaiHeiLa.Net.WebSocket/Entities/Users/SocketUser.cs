using System.Diagnostics;
using System.Globalization;
using KaiHeiLa.Rest;
using Model = KaiHeiLa.API.User;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public abstract class SocketUser : SocketEntity<ulong>, IUser
{
    public abstract string Username { get; internal set; }
    public abstract ushort? IdentifyNumberValue { get; internal set; }
    public abstract bool? IsOnline { get; internal set; }
    public abstract bool? IsBot { get; internal set; }
    public abstract bool? IsBanned { get; internal set; }
    public abstract bool? IsVIP { get; internal set; }
    public abstract string Avatar { get; internal set; }
    public abstract string VIPAvatar { get; internal set; }
    internal abstract SocketGlobalUser GlobalUser { get; }
    
    /// <inheritdoc />
    public string IdentifyNumber => IdentifyNumberValue?.ToString("D4");
    public string KMarkdownMention => MentionUtils.KMarkdownMentionUser(Id);
    public string PlainTextMention => MentionUtils.PlainTextMentionUser(Username, Id);
    
    protected SocketUser(KaiHeiLaSocketClient kaiHeiLa, ulong id) 
        : base(kaiHeiLa, id)
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
        if (model.Online != IsOnline)
        {
            IsOnline = model.Online;
            hasChanges = true;
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
        if (model.IsVIP != IsVIP)
        {
            IsVIP = model.IsVIP;
            hasChanges = true;
        }
        if (model.Avatar != Avatar)
        {
            Avatar = model.Avatar;
            hasChanges = true;
        }
        if (model.VIPAvatar != VIPAvatar)
        {
            VIPAvatar = model.VIPAvatar;
            hasChanges = true;
        }
        
        return hasChanges;
    }
    
    /// <inheritdoc />
    public async Task<IDMChannel> CreateDMChannelAsync(RequestOptions options = null)
        => await UserHelper.CreateDMChannelAsync(this, KaiHeiLa, options).ConfigureAwait(false);

    /// <summary>
    ///     Gets the full name of the user (e.g. Example#0001).
    /// </summary>
    /// <returns>
    ///     The full name of the user.
    /// </returns>
    public override string ToString() => Format.UsernameAndIdentifyNumber(this);
    private string DebuggerDisplay => $"{Format.UsernameAndIdentifyNumber(this)} ({Id}{(IsBot ?? false ? ", Bot" : "")})";
    internal SocketUser Clone() => MemberwiseClone() as SocketUser;
}