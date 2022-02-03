using System.Collections.Concurrent;

namespace KaiHeiLa.WebSocket;

internal class ClientState
{
    // TODO: To be investigated for KaiHeiLa
    private const double AverageChannelsPerGuild = 10.22; //Source: Googie2149
    private const double AverageUsersPerGuild = 47.78; //Source: Googie2149
    private const double CollectionMultiplier = 1.05; //Add 5% buffer to handle growth
    
    private readonly ConcurrentDictionary<ulong, SocketGuild> _guilds;
    
    internal IReadOnlyCollection<SocketGuild> Guilds => _guilds.ToReadOnlyCollection();
    
    public ClientState(int guildCount, int dmChannelCount)
    {
        _guilds = new ConcurrentDictionary<ulong, SocketGuild>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(guildCount * CollectionMultiplier));
    }

    internal SocketGuild GetGuild(ulong id)
    {
        if (_guilds.TryGetValue(id, out SocketGuild guild))
            return guild;
        return null;
    }
    internal void AddGuild(SocketGuild guild)
    {
        _guilds[guild.Id] = guild;
    }
    internal SocketGuild RemoveGuild(ulong id)
    {
        if (_guilds.TryRemove(id, out SocketGuild guild))
        {
            // guild.PurgeChannelCache(this);
            // guild.PurgeUserCache();
            return guild;
        }
        return null;
    }
}