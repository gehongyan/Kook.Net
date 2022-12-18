namespace Kook.Rest;

public static class RestTextChannelExperimentalExtensions
{
    public static Task SyncPermissionsAsync(this RestTextChannel channel, RequestOptions options = null)
        => ExperimentalChannelHelper.SyncPermissionsAsync(channel, channel.Kook, options);
}