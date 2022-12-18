using Kook.API;
using Kook.API.Rest;
using Kook.Net.Queue;

namespace Kook.Rest.Extensions;

internal static class KookRestApiClientExperimentalExtensions
{
    public static async Task SyncChannelPermissionsAsync(this KookRestApiClient client, SyncChannelPermissionsParams args, RequestOptions options = null)
    {
        Preconditions.NotNull(args, nameof(args));
        Preconditions.NotEqual(args.ChannelId, 0, nameof(args.ChannelId));
        options = RequestOptions.CreateOrClone(options);

        var ids = new KookRestApiClient.BucketIds(channelId: args.ChannelId);
        await client.SendJsonAsync(HttpMethod.Post, () => $"channel-role/sync", args, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
    }

}