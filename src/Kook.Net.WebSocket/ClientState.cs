using System.Collections.Concurrent;

namespace Kook.WebSocket;

internal class ClientState
{
    // TODO: To be investigated for Kook
    private const double AverageChannelsPerGuild = 10.22; //Source: Googie2149
    private const double AverageUsersPerGuild = 47.78;    //Source: Googie2149
    private const double CollectionMultiplier = 1.05;     //Add 5% buffer to handle growth

    private readonly ConcurrentDictionary<ulong, SocketChannel> _channels;
    private readonly ConcurrentDictionary<Guid, SocketDMChannel> _dmChannels;
    private readonly ConcurrentDictionary<ulong, SocketGuild> _guilds;
    private readonly ConcurrentDictionary<ulong, SocketGlobalUser> _users;

    internal IReadOnlyCollection<SocketChannel> Channels => _channels.ToReadOnlyCollection();
    internal IReadOnlyCollection<SocketDMChannel> DMChannels => _dmChannels.ToReadOnlyCollection();
    internal IReadOnlyCollection<SocketGuild> Guilds => _guilds.ToReadOnlyCollection();
    internal IReadOnlyCollection<SocketGlobalUser> Users => _users.ToReadOnlyCollection();

    public ClientState(int guildCount, int dmChannelCount)
    {
        double estimatedChannelCount = guildCount * AverageChannelsPerGuild + dmChannelCount;
        double estimatedUsersCount = guildCount * AverageUsersPerGuild;
        _channels = new ConcurrentDictionary<ulong, SocketChannel>(ConcurrentHashSet.DefaultConcurrencyLevel,
            (int)(estimatedChannelCount * CollectionMultiplier));
        _dmChannels = new ConcurrentDictionary<Guid, SocketDMChannel>(ConcurrentHashSet.DefaultConcurrencyLevel,
            (int)(dmChannelCount * CollectionMultiplier));
        _guilds = new ConcurrentDictionary<ulong, SocketGuild>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(guildCount * CollectionMultiplier));
        _users = new ConcurrentDictionary<ulong, SocketGlobalUser>(ConcurrentHashSet.DefaultConcurrencyLevel,
            (int)(estimatedUsersCount * CollectionMultiplier));
    }

    internal SocketChannel? GetChannel(ulong channelId) =>
        _channels.TryGetValue(channelId, out SocketChannel? channel) ? channel : null;

    internal SocketDMChannel? GetDMChannel(Guid userId) =>
        _dmChannels.TryGetValue(userId, out SocketDMChannel? channel) ? channel : null;

    internal SocketDMChannel? GetDMChannel(ulong userId) =>
        _dmChannels.Values.FirstOrDefault(x => x.Recipient.Id == userId);

    internal void AddChannel(SocketChannel channel) => _channels[channel.Id] = channel;

    internal void AddDMChannel(SocketDMChannel channel) => _dmChannels[channel.Id] = channel;

    internal SocketChannel? RemoveChannel(ulong id) =>
        _channels.TryRemove(id, out SocketChannel? channel) ? channel : null;

    internal void PurgeAllChannels()
    {
        foreach (SocketGuild guild in _guilds.Values)
            guild.PurgeChannelCache(this);
    }

    internal void PurgeDMChannels() => _dmChannels.Clear();

    internal SocketGuild? GetGuild(ulong id) =>
        _guilds.TryGetValue(id, out SocketGuild? guild) ? guild : null;

    internal void AddGuild(SocketGuild guild) => _guilds[guild.Id] = guild;

    internal SocketGuild? RemoveGuild(ulong id)
    {
        if (!_guilds.TryRemove(id, out SocketGuild? guild))
            return null;
        guild.PurgeChannelCache(this);
        guild.PurgeUserCache();
        return guild;
    }

    internal SocketGlobalUser? GetUser(ulong id) =>
        _users.TryGetValue(id, out SocketGlobalUser? user) ? user : null;

    internal SocketGlobalUser GetOrAddUser(ulong id, Func<ulong, SocketGlobalUser> userFactory) =>
        _users.GetOrAdd(id, userFactory);

    internal SocketGlobalUser AddOrUpdateUser(ulong id,
        Func<ulong, SocketGlobalUser> addFactory,
        Func<ulong, SocketGlobalUser, SocketGlobalUser> updateFactory) =>
        _users.AddOrUpdate(id, addFactory, updateFactory);

    internal SocketGlobalUser? RemoveUser(ulong id) =>
        _users.TryRemove(id, out SocketGlobalUser? user) ? user : null;

    internal void PurgeUsers()
    {
        foreach (SocketGuild guild in _guilds.Values)
            guild.PurgeUserCache();
    }
}
