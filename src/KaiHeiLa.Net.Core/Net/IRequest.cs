namespace KaiHeiLa.Net
{
    /// <summary>
    ///     Represents a generic request to be sent to KaiHeiLa.
    /// </summary>
    public interface IRequest
    {
        DateTimeOffset? TimeoutAt { get; }
        RequestOptions Options { get; }
    }
}
