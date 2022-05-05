using KaiHeiLa.API.Rest;

namespace KaiHeiLa.WebSocket;

public static class SocketGuildHelper
{
    public static async Task ReloadAsync(SocketGuild guild, KaiHeiLaSocketClient client,
        RequestOptions options)
    {
        ExtendedGuild extendedGuild = await client.ApiClient.GetGuildAsync(guild.Id, options).ConfigureAwait(false);
        guild.Update(client.State, extendedGuild);
    }
}