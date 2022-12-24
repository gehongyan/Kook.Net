using Kook.API.Rest;
using Kook.Rest.Extensions;

namespace Kook.Rest;

internal static class ExperimentalChannelHelper
{
    #region Permissions

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

    #endregion

    #region Voice

    public static async Task DisconnectUserAsync(IVoiceChannel channel, BaseKookClient client, IGuildUser user, RequestOptions options)
    {
        var args = new DisconnectUserParams
        {
            UserId = user.Id,
            ChannelId = channel.Id
        };
        await client.ApiClient.DisconnectUserAsync(args, options).ConfigureAwait(false);
    }

    #endregion

}