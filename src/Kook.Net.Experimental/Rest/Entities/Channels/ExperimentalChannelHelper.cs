using Kook.API.Rest;
using Kook.Rest.Extensions;

namespace Kook.Rest;

internal static class ExperimentalChannelHelper
{
    /// <exception cref="InvalidOperationException">This channel does not have a parent channel.</exception>
    public static async Task SyncPermissionsAsync(INestedChannel channel, BaseKookClient client,
        RequestOptions options)
    {
        var category = await ChannelHelper.GetCategoryAsync(channel, client, options).ConfigureAwait(false);
        if (category == null)
            throw new InvalidOperationException("This channel does not have a parent channel.");

        var args = new SyncChannelPermissionsParams(channel.Id);
        await client.ApiClient.SyncChannelPermissionsAsync(args, options).ConfigureAwait(false);
    }

}