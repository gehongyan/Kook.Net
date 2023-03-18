namespace Kook.Net;

/// <summary>
///     Represents a generic request to be sent to Kook.
/// </summary>
public interface IRequest
{
    /// <summary>
    ///     Gets how long the request should wait before timing out.
    /// </summary>
    DateTimeOffset? TimeoutAt { get; }

    /// <summary>
    ///     Gets the options to be used when sending the request.
    /// </summary>
    RequestOptions Options { get; }
}
