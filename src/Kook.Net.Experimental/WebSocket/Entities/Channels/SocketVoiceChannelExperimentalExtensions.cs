using Kook.Rest;

namespace Kook.WebSocket;

public static class SocketVoiceChannelExperimentalExtensions
{
    public static Task SyncPermissionsAsync(this SocketVoiceChannel channel, RequestOptions options = null)
        => ExperimentalChannelHelper.SyncPermissionsAsync(channel, channel.Kook, options);
}