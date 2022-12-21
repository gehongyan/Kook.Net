using Kook.API;
using Kook.API.Rest;
using Kook.Net.Queue;

namespace Kook.Rest.Extensions;

internal static class KookRestApiClientExperimentalExtensions
{
    #region Guilds

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
    public static async Task<RichGuild> ModifyGuildAsync(this KookRestApiClient client, ulong guildId, ModifyGuildParams args, RequestOptions options = null)
    {
        Preconditions.NotEqual(guildId, 0, nameof(guildId));
        Preconditions.NotNull(args, nameof(args));
        options = RequestOptions.CreateOrClone(options);

        var ids = new KookRestApiClient.BucketIds(guildId: guildId);
        return await client.SendJsonAsync<RichGuild>(HttpMethod.Post, () => $"guild/update", args, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
    }
    public static async Task SyncChannelPermissionsAsync(this KookRestApiClient client, SyncChannelPermissionsParams args, RequestOptions options = null)
    {
        Preconditions.NotNull(args, nameof(args));
        Preconditions.NotEqual(args.ChannelId, 0, nameof(args.ChannelId));
        options = RequestOptions.CreateOrClone(options);

        var ids = new KookRestApiClient.BucketIds(channelId: args.ChannelId);
        await client.SendJsonAsync(HttpMethod.Post, () => $"channel-role/sync", args, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
    }

    #endregion

    #region Voice Regions

    public static IAsyncEnumerable<IReadOnlyCollection<VoiceRegion>> GetVoiceRegionsAsync(this KookRestApiClient client,
        int limit = KookConfig.MaxUsersPerBatch, int fromPage = 1, RequestOptions options = null)
    {
        options = RequestOptions.CreateOrClone(options);
        var ids = new KookRestApiClient.BucketIds();
        PageMeta pageMeta = new(fromPage, limit);
        return client.SendPagedAsync<VoiceRegion>(HttpMethod.Get,(pageSize, page) => $"guild/regions&page_size={pageSize}&page={page}",
            ids, clientBucket: ClientBucketType.SendEdit, pageMeta: pageMeta, options: options);
    }

    #endregion

    #region Voice

    public static async Task DisconnectUserAsync(this KookRestApiClient client, DisconnectUserParams args, RequestOptions options = null)
    {
        Preconditions.NotNull(args, nameof(args));
        Preconditions.NotEqual(args.ChannelId, 0, nameof(args.ChannelId));
        Preconditions.NotEqual(args.UserId, 0, nameof(args.UserId));
        options = RequestOptions.CreateOrClone(options);

        var ids = new KookRestApiClient.BucketIds(channelId: args.ChannelId);
        await client.SendJsonAsync(HttpMethod.Post, () => $"channel/kickout", args, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
    }

    #endregion

}