namespace Kook.Rest;

public static class RestTextChannelExperimentalExtensions
{
    /// <summary>
    ///     Syncs the permissions of this nested channel with its parent's.
    /// </summary>
    /// <param name="channel">The nested channel whose permissions will be synced.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for syncing channel permissions with its parent's.
    /// </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         This method is still in experimental state, which means that it is not for official API implementation
    ///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
    ///     </note>
    /// </remarks>
    public static Task SyncPermissionsAsync(this RestTextChannel channel, RequestOptions options = null)
        => ExperimentalChannelHelper.SyncPermissionsAsync(channel, channel.Kook, options);
}