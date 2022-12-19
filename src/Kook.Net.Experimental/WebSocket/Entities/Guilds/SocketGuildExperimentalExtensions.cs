using Kook.Rest;

namespace Kook.WebSocket;

public static class SocketGuildExperimentalExtensions
{
    public static Task DeleteAsync(this SocketGuild guild, RequestOptions options = null)
        => ExperimentalGuildHelper.DeleteAsync(guild, guild.Kook, options);
}
