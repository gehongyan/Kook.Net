using Kook.Rest.Extensions;

namespace Kook.Rest;

internal static class ExperimentalGuildHelper
{
    public static async Task DeleteAsync(IGuild guild, BaseKookClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteGuildAsync(guild.Id, options).ConfigureAwait(false);
    }
}