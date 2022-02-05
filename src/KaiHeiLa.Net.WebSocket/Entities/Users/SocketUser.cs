using System.Diagnostics;
using System.Globalization;
using Model = KaiHeiLa.API.User;

namespace KaiHeiLa.WebSocket;

/// <summary>
///     Represents a WebSocket-based user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public abstract class SocketUser : SocketEntity<ulong>, IUser
{
    public abstract string Username { get; internal set; }
    public abstract string IdentifyNumber { get; internal set; }
    public abstract ushort IdentifyNumberValue { get; internal set; }
    public abstract bool IsOnline { get; internal set; }
    public abstract bool IsBot { get; internal set; }
    public abstract bool IsBanned { get; internal set; }
    public abstract bool IsVIP { get; internal set; }
    public abstract string Avatar { get; internal set; }
    public abstract string VIPAvatar { get; internal set; }
    internal abstract SocketGlobalUser GlobalUser { get; }
    
    public string Mention => MentionUtils.MentionUser(Id);
    
    protected SocketUser(KaiHeiLaSocketClient kaiHeiLa, ulong id) 
        : base(kaiHeiLa, id)
    {
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
                IdentifyNumber = model.IdentifyNumber;
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
    
    /// <summary>
    ///     Gets the full name of the user (e.g. Example#0001).
    /// </summary>
    /// <returns>
    ///     The full name of the user.
    /// </returns>
    public override string ToString() => Format.UsernameAndDiscriminator(this);
    private string DebuggerDisplay => $"{Format.UsernameAndDiscriminator(this)} ({Id}{(IsBot ? ", Bot" : "")})";
    internal SocketUser Clone() => MemberwiseClone() as SocketUser;
}