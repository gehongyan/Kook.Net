namespace Kook.Rest;

/// <summary>
///     Represents a REST-based channel that is private to select recipients.
/// </summary>
public interface IRestPrivateChannel : IPrivateChannel
{
    /// <summary>
    ///     Users that can access this channel.
    /// </summary>
    new IReadOnlyCollection<RestUser> Recipients { get; }
}
