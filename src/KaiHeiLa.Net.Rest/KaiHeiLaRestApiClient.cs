using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using KaiHeiLa.API;
using KaiHeiLa.Net;
using KaiHeiLa.Net.Queue;
using KaiHeiLa.Net.Rest;
using System.Linq;
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.API;

internal class KaiHeiLaRestApiClient : IDisposable
{
    #region KaiHeiLaRestApiClient
    
    private static readonly ConcurrentDictionary<string, Func<BucketIds, BucketId>> _bucketIdGenerators = new ConcurrentDictionary<string, Func<BucketIds, BucketId>>();

    public event Func<HttpMethod, string, double, Task> SentRequest { add { _sentRequestEvent.Add(value); } remove { _sentRequestEvent.Remove(value); } }
    private readonly AsyncEvent<Func<HttpMethod, string, double, Task>> _sentRequestEvent = new AsyncEvent<Func<HttpMethod, string, double, Task>>();

    protected readonly JsonSerializerOptions SerializerOptions;
    protected readonly SemaphoreSlim _stateLock;
    private readonly RestClientProvider _restClientProvider;

    protected bool _isDisposed;
    private CancellationTokenSource _loginCancelToken;

    public RetryMode DefaultRetryMode { get; }
    public string UserAgent { get; }
    
    internal RequestQueue RequestQueue { get; }
    public LoginState LoginState { get; internal set; }
    public TokenType AuthTokenType { get; set; }
    public string AuthToken { get; set; }
    internal IRestClient RestClient { get; private set; }
    internal ulong? CurrentUserId { get; set; }
    internal Func<IRateLimitInfo, Task> DefaultRatelimitCallback { get; set; }
    
    
    public KaiHeiLaRestApiClient(RestClientProvider restClientProvider, string userAgent, RetryMode defaultRetryMode = RetryMode.AlwaysRetry,
        JsonSerializerOptions serializerOptions = null, Func<IRateLimitInfo, Task> defaultRatelimitCallback = null)
    {
        _restClientProvider = restClientProvider;
        UserAgent = userAgent;
        DefaultRetryMode = defaultRetryMode;
        SerializerOptions = serializerOptions ?? new JsonSerializerOptions {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping};
        DefaultRatelimitCallback = defaultRatelimitCallback;
        
        RequestQueue = new RequestQueue();
        _stateLock = new SemaphoreSlim(1, 1);
        SetBaseUrl(KaiHeiLaConfig.APIUrl);
    }

    internal void SetBaseUrl(string baseUrl)
    
    {
        RestClient?.Dispose();
        RestClient = _restClientProvider(baseUrl);
        RestClient.SetHeader("accept", "*/*");
        RestClient.SetHeader("user-agent", UserAgent);
        RestClient.SetHeader("authorization", GetPrefixedToken(AuthTokenType, AuthToken));
    }
    internal static string GetPrefixedToken(TokenType tokenType, string token)
    {
        return tokenType switch
        {
            TokenType.Bot => $"Bot {token}",
            TokenType.Bearer => $"Bearer {token}",
            _ => throw new ArgumentException(message: "Unknown OAuth token type.", paramName: nameof(tokenType)),
        };
    }
    internal virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
        }
    }
    public void Dispose() => Dispose(true);
    
    public async Task LoginAsync(TokenType tokenType, string token)
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await LoginInternalAsync(tokenType, token).ConfigureAwait(false);
        }
        finally { _stateLock.Release(); }
    }

    private async Task LoginInternalAsync(TokenType tokenType, string token)
    {
        if (LoginState != LoginState.LoggedOut)
            await LogoutInternalAsync().ConfigureAwait(false);
        LoginState = LoginState.LoggingIn;

        try
        {
            _loginCancelToken?.Dispose();
            _loginCancelToken = new CancellationTokenSource();
            
            AuthToken = null;
            await RequestQueue.SetCancelTokenAsync(_loginCancelToken.Token).ConfigureAwait(false);
            RestClient.SetCancelToken(_loginCancelToken.Token);

            AuthTokenType = tokenType;
            AuthToken = token?.TrimEnd();
            RestClient.SetHeader("authorization", GetPrefixedToken(AuthTokenType, AuthToken));

            LoginState = LoginState.LoggedIn;
        }
        catch
        {
            await LogoutInternalAsync().ConfigureAwait(false);
            throw;
        }
    }
    
    public async Task LogoutAsync()
    {
        await _stateLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await LogoutInternalAsync().ConfigureAwait(false);
        }
        finally { _stateLock.Release(); }
    }
    private async Task LogoutInternalAsync()
    {
        //An exception here will lock the client into the unusable LoggingOut state, but that's probably fine since our client is in an undefined state too.
        if (LoginState == LoginState.LoggedOut) return;
        LoginState = LoginState.LoggingOut;

        try { _loginCancelToken?.Cancel(false); }
        catch { }

        await DisconnectInternalAsync(null).ConfigureAwait(false);
        await RequestQueue.ClearAsync().ConfigureAwait(false);

        await RequestQueue.SetCancelTokenAsync(CancellationToken.None).ConfigureAwait(false);
        RestClient.SetCancelToken(CancellationToken.None);

        // CurrentUserId = null;
        LoginState = LoginState.LoggedOut;
    }
    internal virtual Task ConnectInternalAsync() => Task.Delay(0);
    internal virtual Task DisconnectInternalAsync(Exception ex = null) => Task.Delay(0);
    
    #endregion

    #region Core
    
    internal async Task<TResponse> SendAsync<TResponse>(HttpMethod method, Expression<Func<string>> endpointExpr, BucketIds ids,
        ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null) where TResponse : class
        => await SendAsync<TResponse>(method, GetEndpoint(endpointExpr), GetBucketId(method, ids, endpointExpr, funcName), clientBucket, options);
    internal async Task<TResponse> SendAsync<TResponse, TArg1, TArg2>(HttpMethod method, Expression<Func<TArg1, TArg2, string>> endpointExpr, TArg1 arg1, TArg2 arg2, BucketIds ids,
        ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null) where TResponse : class
        => await SendAsync<TResponse>(method, GetEndpoint(endpointExpr, arg1, arg2), GetBucketId(method, ids, endpointExpr, arg1, arg2, funcName), clientBucket, options);
    public async Task<TResponse> SendAsync<TResponse>(HttpMethod method, string endpoint,
        BucketId bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null) where TResponse : class
    {
        options ??= new RequestOptions();
        options.BucketId = bucketId;
        
        var request = new RestRequest(RestClient, method, endpoint, options);
        return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false));
    }

    internal async Task<TResponse> SendJsonAsync<TResponse>(HttpMethod method, Expression<Func<string>> endpointExpr, object payload, BucketIds ids,
        ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null)
        => await SendJsonAsync<TResponse>(method, GetEndpoint(endpointExpr), payload, GetBucketId(method, ids, endpointExpr, funcName), clientBucket, options);
    public async Task<TResponse> SendJsonAsync<TResponse>(HttpMethod method, string endpoint, object payload,
        BucketId bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null)
    {
        options ??= new RequestOptions();
        options.BucketId = bucketId;
        
        string json = payload != null ? SerializeJson(payload) : null;
        
        var request = new JsonRestRequest(RestClient, method, endpoint, json, options);
        return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false));
    }

    internal Task<TResponse> SendMultipartAsync<TResponse>(HttpMethod method, Expression<Func<string>> endpointExpr, IReadOnlyDictionary<string, object> multipartArgs, BucketIds ids,
        ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null, [CallerMemberName] string funcName = null)
        => SendMultipartAsync<TResponse>(method, GetEndpoint(endpointExpr), multipartArgs, GetBucketId(method, ids, endpointExpr, funcName), clientBucket, options);
    public async Task<TResponse> SendMultipartAsync<TResponse>(HttpMethod method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs,
        BucketId bucketId = null, ClientBucketType clientBucket = ClientBucketType.Unbucketed, RequestOptions options = null)
    {
        options ??= new RequestOptions();
        options.BucketId = bucketId;

        var request = new MultipartRestRequest(RestClient, method, endpoint, multipartArgs, options);
        return DeserializeJson<TResponse>(await SendInternalAsync(method, endpoint, request).ConfigureAwait(false));
    }
    
    private async Task<Stream> SendInternalAsync(HttpMethod method, string endpoint, RestRequest request)
    {
        if (!request.Options.IgnoreState)
            CheckState();

        request.Options.RetryMode ??= DefaultRetryMode;
        request.Options.RatelimitCallback ??= DefaultRatelimitCallback;
        
        var stopwatch = Stopwatch.StartNew();
        var responseStream = await RequestQueue.SendAsync(request).ConfigureAwait(false);
        stopwatch.Stop();
        
        double milliseconds = ToMilliseconds(stopwatch);
        await _sentRequestEvent.InvokeAsync(method, endpoint, milliseconds).ConfigureAwait(false);
        
        return responseStream;
    }
    
    /// <param name="endpointExpr">
    ///     <example>
    ///         <c>$"endpoint?params=args&amp;page_size={pageSize}&amp;page={page}"</c>
    ///     </example>
    /// </param>
    private async Task<IReadOnlyCollection<T>> SendPaginationAsync<T>(HttpMethod method, 
        Expression<Func<int, int, string>> endpointExpr,
        BucketIds ids, PageMeta pageMeta = null, RequestOptions options = null)
        where T : class
    {
        List<T> combinedData = new();
        pageMeta ??= PageMeta.Default;
    
        while (pageMeta.Page <= pageMeta.PageTotal)
        {
            PagedResponseBase<T> pagedChannels = await SendAsync<PagedResponseBase<T>, int, int>(
                    method, endpointExpr, pageMeta.PageSize, pageMeta.Page,
                    ids, options: options)
                .ConfigureAwait(false);
            combinedData.AddRange(pagedChannels.Items);
            pageMeta = pagedChannels.Meta;
            pageMeta.Page++;
        }
    
        return combinedData.ToReadOnlyCollection();
    }

    // private async IAsyncEnumerable<IReadOnlyCollection<T>> SendPaginationAsync<T>(HttpMethod method,
    //     Expression<Func<int, int, string>> endpointExpr,
    //     BucketIds ids, PageMeta pageMeta = null, RequestOptions options = null)
    //     where T : class
    // {
    //     pageMeta ??= PageMeta.Default;
    //     
    //     while (pageMeta.Page < pageMeta.PageTotal)
    //     {
    //         PagedResponseBase<T> pagedChannels = await SendAsync<PagedResponseBase<T>, int, int>(
    //                 method, endpointExpr, pageMeta.PageSize, pageMeta.Page,
    //                 ids, options: options)
    //             .ConfigureAwait(false);
    //         pageMeta = pagedChannels.Meta;
    //         yield return pagedChannels.Items;
    //     }
    // }

    #endregion

    #region Guilds

    public async Task<IReadOnlyCollection<Guild>> GetGuildsAsync(RequestOptions options = null)
    {
        options = RequestOptions.CreateOrClone(options);
        
        try
        {
            var ids = new BucketIds();
            return await SendPaginationAsync<Guild>(HttpMethod.Get,
                (pageSize, page) => $"guild/list?page_size={pageSize}&page={page}",
                ids, pageMeta: PageMeta.Default, options: options);
        }
        catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
    }
    
    public async Task<ExtendedGuild> GetGuildAsync(ulong guildId, RequestOptions options = null)
    {
        Preconditions.NotEqual(guildId, 0, nameof(guildId));
        options = RequestOptions.CreateOrClone(options);

        try
        {
            var ids = new BucketIds(guildId: guildId);
            return await SendAsync<ExtendedGuild>(HttpMethod.Get, 
                () => $"guild/view?guild_id={guildId}", ids, options: options).ConfigureAwait(false);
        }
        catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
    }
    
    public async ValueTask<int> GetGuildMemberCountAsync(ulong guildId, RequestOptions options = null)
    {
        Preconditions.NotEqual(guildId, 0, nameof(guildId));
        options = RequestOptions.CreateOrClone(options);

        var ids = new BucketIds(guildId: guildId);
        var response = await SendAsync<GetGuildMemberCountResponse>(HttpMethod.Get, 
            () => $"guild/user-list?guild_id={guildId}", ids, options: options).ConfigureAwait(false);
        return response.UserCount;
    }

    public async Task<IReadOnlyCollection<GuildMember>> GetGuildMembersAsync(ulong guildId, RequestOptions options = null)
    {
        Preconditions.NotEqual(guildId, 0, nameof(guildId));
        options = RequestOptions.CreateOrClone(options);

        try
        {
            var ids = new BucketIds(guildId: guildId);
            return await SendPaginationAsync<GuildMember>(HttpMethod.Get,
                (pageSize, page) => $"guild/user-list?guild_id={guildId}&page={page}",
                ids, pageMeta: new PageMeta(pageSize: 50), options: options);
        }
        catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
    }

    #endregion

    #region Channels

    public async Task<IReadOnlyCollection<Channel>> GetGuildChannelsAsync(ulong guildId, RequestOptions options = null)
    {
        Preconditions.NotEqual(guildId, 0, nameof(guildId));
        options = RequestOptions.CreateOrClone(options);

        try
        {
            var ids = new BucketIds(guildId: guildId);
            return await SendPaginationAsync<Channel>(HttpMethod.Get,
                (pageSize, page) => $"channel/list?guild_id={guildId}&page_size={pageSize}&page={page}",
                ids, pageMeta: PageMeta.Default, options: options);
        }
        catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.NotFound) { return null; }
    }

    #endregion

    #region Messages

    public async Task<CreateMessageResponse> CreateMessageAsync(CreateMessageParams args, RequestOptions options = null)
    {
        Preconditions.NotNull(args, nameof(args));
        Preconditions.NotEqual(args.ChannelId, 0, nameof(args.ChannelId));
        
        if (args.Content?.Length > KaiHeiLaConfig.MaxMessageSize)
            throw new ArgumentException(message: $"Message content is too long, length must be less or equal to {KaiHeiLaConfig.MaxMessageSize}.", paramName: nameof(args.Content));
        options = RequestOptions.CreateOrClone(options);
        
        var ids = new BucketIds(channelId: args.ChannelId);
        return await SendJsonAsync<CreateMessageResponse>(HttpMethod.Post, () => $"message/create", args, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
    }

    #endregion

    #region DM Channels

    

    #endregion

    #region DM Messages

    

    #endregion
    
    #region Gateway
    
    public async Task<GetGatewayResponse> GetGatewayAsync(RequestOptions options = null)
    {
        options = RequestOptions.CreateOrClone(options);
        return await SendAsync<GetGatewayResponse>(HttpMethod.Get, () => "gateway/index?compress=0", new BucketIds(), options: options).ConfigureAwait(false);
    }

    #endregion
    
    #region Users
    
    public async Task<SelfUser> GetSelfUserAsync(RequestOptions options = null)
    {
        options = RequestOptions.CreateOrClone(options);
        return await SendAsync<SelfUser>(HttpMethod.Get, () => "user/me", new BucketIds(), options: options).ConfigureAwait(false);
    }

    #endregion

    #region Assets

    public async Task<CreateAssetResponse> CreateAssetAsync(string path, string fileName = null, RequestOptions options = null)
    {
        Preconditions.NotNull(path, nameof(path));
        options = RequestOptions.CreateOrClone(options);
        
        Dictionary<string, object> dictionary = new();
        await using FileStream fileStream = File.OpenRead(path);
        dictionary["file"] = new MultipartFile(fileStream, fileName ?? Path.GetFileName(path));
        
        var ids = new BucketIds();
        return await SendMultipartAsync<CreateAssetResponse>(HttpMethod.Post, () => $"asset/create", dictionary, ids, clientBucket: ClientBucketType.SendEdit, options: options).ConfigureAwait(false);
    }

    #endregion

    #region Guild Roles

    

    #endregion

    #region Intimacy

    

    #endregion

    #region Guild Emoji

    

    #endregion

    #region Guild Invites

    

    #endregion

    #region Guild Bans

    

    #endregion

    #region Badges

    

    #endregion
    
    
    #region Helpers

    protected void CheckState()
    {
        if (LoginState != LoginState.LoggedIn)
            throw new InvalidOperationException("Client is not logged in.");
    }
    protected static double ToMilliseconds(Stopwatch stopwatch) => Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
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

    internal class BucketIds
    {
        public ulong GuildId { get; internal set; }
        public ulong ChannelId { get; internal set; }
        public HttpMethod HttpMethod { get; internal set; }

        internal BucketIds(ulong guildId = 0, ulong channelId = 0)
        {
            GuildId = guildId;
            ChannelId = channelId;
        }

        internal object[] ToArray()
            => new object[] { HttpMethod, GuildId, ChannelId };

        internal Dictionary<string, string> ToMajorParametersDictionary()
        {
            var dict = new Dictionary<string, string>();
            if (GuildId != 0)
                dict["GuildId"] = GuildId.ToString();
            if (ChannelId != 0)
                dict["ChannelId"] = ChannelId.ToString();
            return dict;
        }

        internal static int? GetIndex(string name)
        {
            return name switch
            {
                "httpMethod" => 0,
                "guildId" => 1,
                "channelId" => 2,
                _ => null,
            };
        }
    }
    
    private static string GetEndpoint(Expression<Func<string>> endpointExpr)
    {
        return endpointExpr.Compile()();
    }
    private static string GetEndpoint<T1, T2>(Expression<Func<T1, T2, string>> endpointExpr, T1 arg1, T2 arg2)
    {
        return endpointExpr.Compile()(arg1, arg2);
    }
    private static BucketId GetBucketId(HttpMethod httpMethod, BucketIds ids, Expression<Func<string>> endpointExpr, string callingMethod)
    {
        ids.HttpMethod = httpMethod;
        return _bucketIdGenerators.GetOrAdd(callingMethod, x => CreateBucketId(endpointExpr))(ids);
    }
    private static BucketId GetBucketId<TArg1, TArg2>(HttpMethod httpMethod, BucketIds ids, Expression<Func<TArg1, TArg2, string>> endpointExpr, TArg1 arg1, TArg2 arg2, string callingMethod)
    {
        ids.HttpMethod = httpMethod;
        return _bucketIdGenerators.GetOrAdd(callingMethod, x => CreateBucketId(endpointExpr, arg1, arg2))(ids);
    }

    private static Func<BucketIds, BucketId> CreateBucketId<TArg1, TArg2>(Expression<Func<TArg1, TArg2, string>> endpoint, TArg1 arg1, TArg2 arg2)
    {
        return CreateBucketId(() => endpoint.Compile().Invoke(arg1, arg2));
    }

    private static Func<BucketIds, BucketId> CreateBucketId(Expression<Func<string>> endpoint)
    {
        try
        {
            //Is this a constant string?
            if (endpoint.Body.NodeType == ExpressionType.Constant)
                return x => BucketId.Create(x.HttpMethod, (endpoint.Body as ConstantExpression).Value.ToString(), x.ToMajorParametersDictionary());

            var builder = new StringBuilder();
            var methodCall = endpoint.Body as MethodCallExpression;
            var methodArgs = methodCall.Arguments.ToArray();
            string format = methodArgs[0].NodeType == ExpressionType.Constant
                ? (methodArgs[0] as ConstantExpression).Value as string
                : endpoint.Compile()();

            //Unpack the array, if one exists (happens with 4+ parameters)
            if (methodArgs.Length > 1 && methodArgs[1].NodeType == ExpressionType.NewArrayInit)
            {
                var arrayExpr = methodArgs[1] as NewArrayExpression;
                var elements = arrayExpr.Expressions.ToArray();
                Array.Resize(ref methodArgs, elements.Length + 1);
                Array.Copy(elements, 0, methodArgs, 1, elements.Length);
            }

            int endIndex = format.IndexOf('?'); //Don't include params
            if (endIndex == -1)
                endIndex = format.Length;

            int lastIndex = 0;
            while (true)
            {
                int leftIndex = format.IndexOf("{", lastIndex);
                if (leftIndex == -1 || leftIndex > endIndex)
                {
                    builder.Append(format, lastIndex, endIndex - lastIndex);
                    break;
                }
                builder.Append(format, lastIndex, leftIndex - lastIndex);
                int rightIndex = format.IndexOf("}", leftIndex);

                int argId = int.Parse(format.Substring(leftIndex + 1, rightIndex - leftIndex - 1), NumberStyles.None, CultureInfo.InvariantCulture);
                string fieldName = GetFieldName(methodArgs[argId + 1]);

                var mappedId = BucketIds.GetIndex(fieldName);

                if (!mappedId.HasValue && rightIndex != endIndex && format.Length > rightIndex + 1 && format[rightIndex + 1] == '/') //Ignore the next slash
                    rightIndex++;

                if (mappedId.HasValue)
                    builder.Append($"{{{mappedId.Value}}}");

                lastIndex = rightIndex + 1;
            }
            if (builder[builder.Length - 1] == '/')
                builder.Remove(builder.Length - 1, 1);

            format = builder.ToString();

            return x => BucketId.Create(x.HttpMethod, string.Format(format, x.ToArray()), x.ToMajorParametersDictionary());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to generate the bucket id for this operation.", ex);
        }
    }

    private static string GetFieldName(Expression expr)
    {
        if (expr.NodeType == ExpressionType.Convert)
            expr = (expr as UnaryExpression).Operand;

        if (expr.NodeType != ExpressionType.MemberAccess)
            throw new InvalidOperationException("Unsupported expression");

        return (expr as MemberExpression).Member.Name;
    }

    #endregion
}