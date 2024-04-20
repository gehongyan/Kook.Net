#if NET462
using System.Net.Http;
#endif

using Kook.Net.Rest;

namespace Kook.Net.Queue;

internal class MultipartRestRequest : RestRequest
{
    public IReadOnlyDictionary<string, object> MultipartParams { get; }

    public MultipartRestRequest(IRestClient client, HttpMethod method, string endpoint,
        IReadOnlyDictionary<string, object> multipartParams, RequestOptions options)
        : base(client, method, endpoint, options)
    {
        MultipartParams = multipartParams;
    }

    public override async Task<RestResponse> SendAsync() => await Client
        .SendAsync(Method, Endpoint, MultipartParams, Options.CancellationToken,
            Options.AuditLogReason, Options.RequestHeaders)
        .ConfigureAwait(false);
}
