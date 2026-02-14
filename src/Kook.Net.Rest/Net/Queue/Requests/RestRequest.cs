using Kook.Net.Rest;

namespace Kook.Net.Queue;

internal class RestRequest : IRequest
{
    public IRestClient Client { get; }
    public HttpMethod Method { get; }
    public string Endpoint { get; }
    public DateTimeOffset? TimeoutAt { get; }
    public TaskCompletionSource<Stream> Promise { get; }
    public RequestOptions Options { get; }

    public RestRequest(IRestClient client, HttpMethod method, string endpoint, RequestOptions? options)
    {
        Preconditions.NotNull(options, nameof(options));

        Client = client;
        Method = method;
        Endpoint = endpoint;
        Options = options;
        TimeoutAt = options.Timeout.HasValue ? DateTimeOffset.UtcNow.AddMilliseconds(options.Timeout.Value) : null;
        Promise = new TaskCompletionSource<Stream>();
    }

    public virtual async Task<RestResponse> SendAsync() =>
        await Client.SendAsync(Method, Endpoint, Options.CancellationToken, Options.AuditLogReason, Options.RequestHeaders)
            .ConfigureAwait(false);
}
