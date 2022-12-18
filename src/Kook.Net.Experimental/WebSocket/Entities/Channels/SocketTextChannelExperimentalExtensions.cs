using Kook.Rest;

namespace Kook.WebSocket;

public static class SocketTextChannelExperimentalExtensions
{
    public static Task SyncPermissionsAsync(this SocketTextChannel channel, RequestOptions options = null)
        => ExperimentalChannelHelper.SyncPermissionsAsync(channel, channel.Kook, options);
}