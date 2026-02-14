using Kook.Net.Rest;

namespace Kook.Net.Queue;

internal class JsonRestRequest : RestRequest
{
    public string Json { get; }

    public JsonRestRequest(IRestClient client, HttpMethod method, string endpoint, string json, RequestOptions? options)
        : base(client, method, endpoint, options)
    {
        Json = json;
    }

    public override async Task<RestResponse> SendAsync() => await Client
        .SendAsync(Method, Endpoint, Json, Options.CancellationToken, Options.AuditLogReason, Options.RequestHeaders)
        .ConfigureAwait(false);
}
