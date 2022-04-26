using KaiHeiLa.Net.Rest;

namespace KaiHeiLa.Net.Queue
{
    internal class JsonRestRequest : RestRequest
    {
        public string Json { get; }

        public JsonRestRequest(IRestClient client, HttpMethod method, string endpoint, string json, RequestOptions options)
            : base(client, method, endpoint, options)
        {
            Json = json;
        }

        public override async Task<RestResponse> SendAsync()
        {
            return await Client.SendAsync(Method, Endpoint, Json, Options.CancelToken).ConfigureAwait(false);
        }
    }
}
