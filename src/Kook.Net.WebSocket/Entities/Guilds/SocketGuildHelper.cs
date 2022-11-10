using System.Collections.Immutable;
using Kook.API.Rest;
using Kook.Rest;

namespace Kook.WebSocket;

public static class SocketGuildHelper
{
    public static async Task UpdateAsync(SocketGuild guild, KookSocketClient client,
        RequestOptions options)
    {
        ExtendedGuild extendedGuild = await client.ApiClient.GetGuildAsync(guild.Id, options).ConfigureAwait(false);
        if (client.AlwaysDownloadBoostSubscriptions && guild.BufferBoostSubscriptionCount != extendedGuild.BufferBoostSubscriptionCount)
            await guild.DownloadBoostSubscriptionsAsync();
        guild.Update(client.State, extendedGuild);
    }

    public static async Task<ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>> GetBoostSubscriptionsAsync(
        SocketGuild guild, BaseSocketClient client, RequestOptions options)
    {
        IEnumerable<BoostSubscription> subscriptions = await client.ApiClient
            .GetGuildBoostSubscriptionsAsync(guild.Id, options: options).FlattenAsync();
        return subscriptions.GroupBy(x => x.UserId)
            .ToImmutableDictionary(x => guild.GetUser(x.Key) ?? client.GetUser(x.Key) ?? RestUser.Create(client, x.First().User) as IUser,
                x => x.GroupBy(y => (y.StartTime, y.EndTime))
                    .Select(y => new BoostSubscriptionMetadata(y.Key.StartTime, y.Key.EndTime, y.Count()))
                    .ToImmutableArray() as IReadOnlyCollection<BoostSubscriptionMetadata>);
    }

}