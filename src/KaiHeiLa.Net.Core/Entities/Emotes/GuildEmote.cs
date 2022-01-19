using System.Globalization;

namespace KaiHeiLa;

public class GuildEmote : Emote
{
    internal GuildEmote(string id, string name, bool animated, ulong guidId, IUser creator) 
        : base(id, name, animated)
    {
        GuidId = guidId;
        Creator = creator;
    }

    public ulong GuidId { get; }

    public IUser Creator { get; }
}