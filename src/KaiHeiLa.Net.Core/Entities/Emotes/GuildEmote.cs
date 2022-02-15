using System.Globalization;

namespace KaiHeiLa;

public class GuildEmote : Emote
{
    internal GuildEmote(string id, string name, bool animated, ulong guildId, IUser creator) 
        : base(id, name, animated)
    {
        GuildId = guildId;
        Creator = creator;
    }

    public ulong GuildId { get; }

    public IUser Creator { get; }
}