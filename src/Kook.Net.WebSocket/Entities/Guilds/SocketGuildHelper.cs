using Kook.API.Rest;

namespace Kook.WebSocket;

public static class SocketGuildHelper
{
    public static async Task UpdateAsync(SocketGuild guild, KookSocketClient client,
        RequestOptions options)
    {
        ExtendedGuild extendedGuild = await client.ApiClient.GetGuildAsync(guild.Id, options).ConfigureAwait(false);
        guild.Update(client.State, extendedGuild);
    }
}