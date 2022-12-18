namespace Kook.Rest;

public static class RestVoiceChannelExperimentalExtensions
{
    public static Task SyncPermissionsAsync(this RestVoiceChannel channel, RequestOptions options = null)
        => ExperimentalChannelHelper.SyncPermissionsAsync(channel, channel.Kook, options);
}