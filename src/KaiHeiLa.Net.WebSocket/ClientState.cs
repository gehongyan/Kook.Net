using System.Collections.Concurrent;

namespace KaiHeiLa.WebSocket;

internal class ClientState
{
    // TODO: To be investigated for KaiHeiLa
    private const double AverageChannelsPerGuild = 10.22; //Source: Googie2149
    private const double AverageUsersPerGuild = 47.78; //Source: Googie2149
    private const double CollectionMultiplier = 1.05; //Add 5% buffer to handle growth
    
    private readonly ConcurrentDictionary<ulong, SocketChannel> _guildChannels;
    private readonly ConcurrentDictionary<Guid, SocketDMChannel> _dmChannels;
    private readonly ConcurrentDictionary<ulong, SocketGuild> _guilds;
    private readonly ConcurrentDictionary<ulong, SocketGlobalUser> _users;
    
    internal IReadOnlyCollection<SocketChannel> GuildChannels => _guildChannels.ToReadOnlyCollection();
    internal IReadOnlyCollection<SocketDMChannel> DMChannels => _dmChannels.ToReadOnlyCollection();
    internal IReadOnlyCollection<SocketGuild> Guilds => _guilds.ToReadOnlyCollection();
    internal IReadOnlyCollection<SocketGlobalUser> Users => _users.ToReadOnlyCollection();
    
    public ClientState(int guildCount, int dmChannelCount)
    {
        double estimatedChannelCount = guildCount * AverageChannelsPerGuild + dmChannelCount;
        double estimatedUsersCount = guildCount * AverageUsersPerGuild;
        _guildChannels = new ConcurrentDictionary<ulong, SocketChannel>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(estimatedChannelCount * CollectionMultiplier));
        _dmChannels = new ConcurrentDictionary<Guid, SocketDMChannel>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(dmChannelCount * CollectionMultiplier));
        _guilds = new ConcurrentDictionary<ulong, SocketGuild>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(guildCount * CollectionMultiplier));
        _users = new ConcurrentDictionary<ulong, SocketGlobalUser>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(estimatedUsersCount * CollectionMultiplier));
    }
    
    internal SocketChannel GetChannel(ulong id)
    {
        if (_guildChannels.TryGetValue(id, out SocketChannel channel))
            return channel;
        return null;
    }
    internal SocketDMChannel GetDMChannel(Guid userId)
    {
        if (_dmChannels.TryGetValue(userId, out SocketDMChannel channel))
            return channel;
        return null;
    }
    internal void AddChannel(SocketChannel channel)
    {
        _guildChannels[channel.Id] = channel;
    }
    internal void AddDMChannel(SocketDMChannel channel)
    {
        _dmChannels[channel.Id] = channel;
    }
    internal SocketChannel RemoveChannel(ulong id)
    {
        if (_guildChannels.TryRemove(id, out SocketChannel channel))
        {
            return channel;
        }
        return null;
    }
    internal void PurgeAllChannels()
    {
        foreach (var guild in _guilds.Values)
            guild.PurgeChannelCache(this);
    }
    internal void PurgeDMChannels()
    {
        _dmChannels.Clear();
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
            guild.PurgeChannelCache(this);
            guild.PurgeUserCache();
            return guild;
        }
        return null;
    }
    
    internal SocketGlobalUser GetUser(ulong id)
    {
        if (_users.TryGetValue(id, out SocketGlobalUser user))
            return user;
        return null;
    }
    internal SocketGlobalUser GetOrAddUser(ulong id, Func<ulong, SocketGlobalUser> userFactory)
    {
        return _users.GetOrAdd(id, userFactory);
    }
    internal SocketGlobalUser RemoveUser(ulong id)
    {
        if (_users.TryRemove(id, out SocketGlobalUser user))
            return user;
        return null;
    }
    internal void PurgeUsers()
    {
        foreach (var guild in _guilds.Values)
            guild.PurgeUserCache();
    }
    
}