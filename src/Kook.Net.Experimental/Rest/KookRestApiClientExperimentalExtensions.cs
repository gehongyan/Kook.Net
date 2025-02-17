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
}
