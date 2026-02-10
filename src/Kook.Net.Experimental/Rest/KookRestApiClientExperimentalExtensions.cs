#if NET462
using System.Net.Http;
#endif

using Kook.API;
using Kook.API.Rest;
using Kook.Net.Queue;

namespace Kook.Rest.Extensions;

internal static class KookRestApiClientExperimentalExtensions
{
    #region Guilds

    public static IAsyncEnumerable<IReadOnlyCollection<Guild>> GetAdminGuildsAsync(this KookRestApiClient client,
        int limit = KookConfig.MaxItemsPerBatchByDefault, int fromPage = 1, RequestOptions? options = null)
    {
        options = RequestOptions.CreateOrClone(options);
        KookRestApiClient.BucketIds ids = new();
        return client.SendPagedAsync<Guild>(HttpMethod.Get,
            (pageSize, page) => $"guild/list?page_size={pageSize}&page={page}&type=admin",
            ids, ClientBucketType.SendEdit, new PageMeta(fromPage, limit), options);
    }

    public static IAsyncEnumerable<IReadOnlyCollection<GuildSecurityItem>> GetGuildSecurityItemsAsync(this KookRestApiClient client,
        ulong guildId, int limit = 50, int fromPage = 1, RequestOptions? options = null)
    {
        options = RequestOptions.CreateOrClone(options);
        KookRestApiClient.BucketIds ids = new(guildId);
        return client.SendPagedAsync<GuildSecurityItem>(HttpMethod.Get,
            (pageSize, page) => $"guild-security?guild_id={guildId}&page_size={pageSize}&page={page}",
            ids, ClientBucketType.SendEdit, new PageMeta(fromPage, limit), options);
    }

    #endregion

    #region Messages

    public static async Task ValidateCardsAsync(this KookRestApiClient client,
        ValidateCardsParams args, RequestOptions? options = null)
    {
        Preconditions.NotNull(args, nameof(args));
        Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
        options = RequestOptions.CreateOrClone(options);

        KookRestApiClient.BucketIds ids = new();
        await client.SendJsonAsync(HttpMethod.Post,
                () => $"message/check-card", args, ids, ClientBucketType.Unbucketed, null, options)
            .ConfigureAwait(false);
    }

    #endregion

    #region Threads

    public static async Task<IReadOnlyCollection<API.Rest.ThreadTag>> QueryThreadTagsAsync(this KookRestApiClient client,
        string keyword, RequestOptions? options = null)
    {
        Preconditions.NotNullOrEmpty(keyword, nameof(keyword));
        options = RequestOptions.CreateOrClone(options);

        KookRestApiClient.BucketIds ids = new();
        string escapedKeyword = Uri.EscapeDataString(keyword);
        return await client.SendAsync<IReadOnlyCollection<API.Rest.ThreadTag>>(HttpMethod.Get,
                () => $"guild-recommend-tag/thread-index?name={escapedKeyword}",
                ids, ClientBucketType.Unbucketed, false, options)
            .ConfigureAwait(false);
    }

    #endregion
}
