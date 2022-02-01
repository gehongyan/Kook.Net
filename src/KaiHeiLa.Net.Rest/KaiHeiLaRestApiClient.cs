using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;
using KaiHeiLa.API;

namespace KaiHeiLa.API;

internal class KaiHeiLaRestApiClient
{
    internal readonly JsonSerializerOptions SerializerOptions = new()
        { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public KaiHeiLaRestApiClient()
    {
        
    }

    public TokenType TokenType { get; set; }
    public string AuthToken { get; set; }
    public LoginState LoginState { get; internal set; }

    public void SetAuthToken(TokenType tokenType, string authToken)
    {
        TokenType = tokenType;
        AuthToken = authToken;
    }
    
    /// <summary>
    ///     获取网关地址
    /// </summary>
    public async Task<RestResponse<GetGatewayResponse>> GetGatewayAsync()
    {
        return await SendAsync<RestResponse<GetGatewayResponse>>(HttpMethod.Get, () => "gateway/index?compress=0").ConfigureAwait(false);
    }

    /// <summary>
    ///     不带负载发送请求
    /// </summary>
    /// <param name="method"></param>
    /// <param name="endpointExpr"></param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    internal async Task<TResponse> SendAsync<TResponse>(HttpMethod method, Expression<Func<string>> endpointExpr) where TResponse : class
        => await SendAsync<TResponse>(method, GetEndpoint(endpointExpr));
    public async Task<TResponse> SendAsync<TResponse>(HttpMethod method, string endpoint) where TResponse : class
    {
        return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint).ConfigureAwait(false));
    }

    /// <summary>
    ///     带负载发送请求
    /// </summary>
    /// <param name="method"></param>
    /// <param name="endpointExpr"></param>
    /// <param name="payload"></param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    internal async Task<TResponse> SendJsonAsync<TResponse>(HttpMethod method, Expression<Func<string>> endpointExpr, object payload)
        => await SendJsonAsync<TResponse>(method, GetEndpoint(endpointExpr), payload);
    public async Task<TResponse> SendJsonAsync<TResponse>(HttpMethod method, string endpoint, object payload)
    {
        return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, SerializeJson(payload)).ConfigureAwait(false));
    }

    private async Task<Stream> SendInternalAsync(HttpMethod method, string endpoint, string json = null)
    {
        HttpRequestMessage request = new(method, $"{KaiHeiLaConfig.APIUrl}{endpoint}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bot", AuthToken);
        if (!string.IsNullOrWhiteSpace(json))
        {
            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        }
        HttpClient httpClient = new();
        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);
        return await httpResponseMessage.Content.ReadAsStreamAsync();
    }

    protected string SerializeJson(object payload)
    {
        return payload is null 
            ? string.Empty 
            : JsonSerializer.Serialize(payload, SerializerOptions);
    }
    
    protected T DeserializeJson<T>(Stream jsonStream)
    {
        return JsonSerializer.Deserialize<T>(jsonStream, SerializerOptions);
    }

    private static string GetEndpoint(Expression<Func<string>> endpointExpr)
    {
        return endpointExpr.Compile()();
    }
    protected void CheckState()
    {
        if (LoginState != LoginState.LoggedIn)
            throw new InvalidOperationException("Client is not logged in.");
    }
    

}