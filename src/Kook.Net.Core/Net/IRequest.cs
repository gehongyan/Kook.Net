namespace Kook.Net;

/// <summary>
///     Represents a generic request to be sent to Kook.
/// </summary>
public interface IRequest
{
    DateTimeOffset? TimeoutAt { get; }
    RequestOptions Options { get; }
}
