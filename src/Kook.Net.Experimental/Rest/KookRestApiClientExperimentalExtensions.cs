using Kook.API;
using Kook.API.Rest;
using Kook.Net.Queue;

namespace Kook.Rest.Extensions;

internal static class KookRestApiClientExperimentalExtensions
{
    public static async Task<RichGuild> CreateGuildAsync(this KookRestApiClient client, CreateGuildParams args, RequestOptions options = null)
    {
        Preconditions.NotNull(args, nameof(args));
        Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));
        options = RequestOptions.CreateOrClone(options);

        var ids = new KookRestApiClient.BucketIds();
        return await client.SendJsonAsync<RichGuild>(HttpMethod.Post, () => "guild/create", args, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
    }
    public static async Task DeleteGuildAsync(this KookRestApiClient client, DeleteGuildParams args, RequestOptions options = null)
    {
        Preconditions.NotNull(args, nameof(args));
        Preconditions.NotEqual(args.GuildId, 0, nameof(args.GuildId));
        options = RequestOptions.CreateOrClone(options);

        var ids = new KookRestApiClient.BucketIds(guildId: args.GuildId);
        await client.SendJsonAsync(HttpMethod.Post, () => $"guild/delete", args, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
    }
    public static async Task SyncChannelPermissionsAsync(this KookRestApiClient client, SyncChannelPermissionsParams args, RequestOptions options = null)
    {
        Preconditions.NotNull(args, nameof(args));
        Preconditions.NotEqual(args.ChannelId, 0, nameof(args.ChannelId));
        options = RequestOptions.CreateOrClone(options);

        var ids = new KookRestApiClient.BucketIds(channelId: args.ChannelId);
        await client.SendJsonAsync(HttpMethod.Post, () => $"channel-role/sync", args, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
    }

}