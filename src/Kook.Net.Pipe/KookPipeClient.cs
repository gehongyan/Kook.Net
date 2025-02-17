﻿using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.Logging;
using Kook.Rest;

#if NET462
using System.Net.Http;
#endif

namespace Kook.Pipe;

/// <summary>
///     表示一个用于管道通信的客户端。
/// </summary>
public class KookPipeClient : IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    ///     当生成一条日志消息时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="Kook.LogMessage"/> 参数是描述日志消息的结构。 </item>
    ///     </list>
    /// </remarks>
    public event Func<LogMessage, Task> Log
    {
        add => _logEvent.Add(value);
        remove => _logEvent.Remove(value);
    }

    internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new();

    /// <summary>
    ///     当向 API 发送 REST 请求时引发。
    /// </summary>
    /// <remarks>
    ///     事件参数：
    ///     <list type="number">
    ///     <item> <see cref="System.Net.Http.HttpMethod"/> 参数是 HTTP 方法。 </item>
    ///     <item> <see cref="System.String"/> 参数是终结点。 </item>
    ///     <item> <see cref="System.Double"/> 参数是完成请求所花费的时间，以毫秒为单位。 </item>
    ///     </list>
    /// </remarks>
    public event Func<HttpMethod, string, double, Task> SentRequest
    {
        add => _sentRequest.Add(value);
        remove => _sentRequest.Remove(value);
    }

    internal readonly AsyncEvent<Func<HttpMethod, string, double, Task>> _sentRequest = new();

    private readonly string _accessToken;

    internal API.KookRestApiClient ApiClient { get; }
    internal LogManager LogManager { get; }

    /// <summary>
    ///     初始化一个 <see cref="KookPipeClient"/> 类的新实例。
    /// </summary>
    /// <param name="accessToken"> 用于管道通信的访问令牌。 </param>
    public KookPipeClient(string accessToken)
        : this(accessToken, new KookRestConfig())
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="KookPipeClient"/> 类的新实例。
    /// </summary>
    /// <param name="accessToken"> 用于管道通信的访问令牌。 </param>
    /// <param name="config"> 用于配置客户端的配置。 </param>
    public KookPipeClient(string accessToken, KookRestConfig config)
        : this(config)
    {
        ApiClient.LoginAsync(TokenType.Pipe, accessToken).GetAwaiter().GetResult();
        _accessToken = accessToken;
    }

    /// <summary>
    ///     初始化一个 <see cref="KookPipeClient"/> 类的新实例。
    /// </summary>
    /// <param name="pipeUrl"> 用于管道通信的 URL。 </param>
    public KookPipeClient(Uri pipeUrl)
        : this(pipeUrl, new KookRestConfig())
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="KookPipeClient"/> 类的新实例。
    /// </summary>
    /// <param name="pipeUrl"> 用于管道通信的 URL。 </param>
    /// <param name="config"> 用于配置客户端的配置。 </param>
    public KookPipeClient(Uri pipeUrl, KookRestConfig config)
        : this(config)
    {
        if (!ParsePipeUrl(pipeUrl, out string? accessToken))
            throw new ArgumentException("The pipe URL is invalid.", nameof(pipeUrl));
        ApiClient.LoginAsync(TokenType.Pipe, accessToken).GetAwaiter().GetResult();
        _accessToken = accessToken;
    }

    private KookPipeClient(KookRestConfig config)
    {
        _accessToken = string.Empty;
        ApiClient = CreateApiClient(config);
        LogManager = new LogManager(config.LogLevel);
        LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);

        Logger restLogger = LogManager.CreateLogger("Rest");
        ApiClient.RequestQueue.RateLimitTriggered += async (id, info, endpoint) =>
        {
            if (info == null)
                await restLogger.VerboseAsync($"Preemptive Rate limit triggered: {endpoint} {(id.IsHashBucket ? $"(Bucket: {id.BucketHash})" : "")}").ConfigureAwait(false);
            else
                await restLogger.WarningAsync($"Rate limit triggered: {endpoint} {(id.IsHashBucket ? $"(Bucket: {id.BucketHash})" : "")}").ConfigureAwait(false);
        };
        ApiClient.SentRequest += async (method, endpoint, millis) =>
            await restLogger.VerboseAsync($"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
        ApiClient.SentRequest += (method, endpoint, millis) =>
            _sentRequest.InvokeAsync(method, endpoint, millis);
    }

    private static API.KookRestApiClient CreateApiClient(KookRestConfig config) =>
        new(config.RestClientProvider, KookConfig.UserAgent, config.AcceptLanguage,
            config.DefaultRetryMode, SerializerOptions);

    /// <summary>
    ///     发送文本消息内容到管道。
    /// </summary>
    /// <param name="text"> 要发送的消息文本。 </param>
    /// <param name="options"> 用于配置请求的选项。 </param>
    /// <returns> 返回一个表示异步操作的任务，任务的结果是消息的 ID。 </returns>
    [Obsolete("This method is obsolete. Use SendTextAsync instead.")]
    public Task<Guid> SendContentAsync(string text, RequestOptions? options = null) =>
        SendTextAsync(text, null, null, options);

    /// <summary>
    ///     发送文本消息内容到管道。
    /// </summary>
    /// <param name="parameters"> 要应用到消息管道所使用的消息模板的参数。 </param>
    /// <param name="jsonSerializerOptions"> 序列化模板参数时要使用的序列化选项。 </param>
    /// <param name="options"> 用于配置请求的选项。 </param>
    /// <returns> 返回一个表示异步操作的任务，任务的结果是消息的 ID。 </returns>
    [Obsolete("This method is obsolete. Use SendTextAsync instead.")]
    public Task<Guid> SendTemplateAsync<T>(T? parameters = default,
        JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        SendTextAsync(parameters, null, null, jsonSerializerOptions, options);

    /// <summary>
    ///     发送文本消息内容到管道。
    /// </summary>
    /// <param name="text"> 要发送的消息文本。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUserId"> 临时消息的接收者的用户 ID。如果设置为指定的用户 ID，则仅该 ID 所关联的用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="options"> 用于配置请求的选项。 </param>
    /// <returns> 返回一个表示异步操作的任务，任务的结果是消息的 ID。 </returns>
    [OverloadResolutionPriority(1)]
    public Task<Guid> SendTextAsync(string text, IQuote? quote = null,
        ulong? ephemeralUserId = null, RequestOptions? options = null) =>
        PipeClientHelper.SendTextAsync(this, text, quote, ephemeralUserId, options);

    /// <summary>
    ///     发送文本消息内容到管道。
    /// </summary>
    /// <param name="parameters"> 要应用到消息管道所使用的消息模板的参数。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUserId"> 临时消息的接收者的用户 ID。如果设置为指定的用户 ID，则仅该 ID 所关联的用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="jsonSerializerOptions"> 序列化模板参数时要使用的序列化选项。 </param>
    /// <param name="options"> 用于配置请求的选项。 </param>
    /// <returns> 返回一个表示异步操作的任务，任务的结果是消息的 ID。 </returns>
    public Task<Guid> SendTextAsync<T>(T? parameters = default, IQuote? quote = null,
        ulong? ephemeralUserId = null, JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        PipeClientHelper.SendTextAsync(this, parameters, quote, ephemeralUserId, jsonSerializerOptions, options);

    /// <summary>
    ///     发送卡片消息内容到管道。
    /// </summary>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUserId"> 临时消息的接收者的用户 ID。如果设置为指定的用户 ID，则仅该 ID 所关联的用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="options"> 用于配置请求的选项。 </param>
    /// <returns> 返回一个表示异步操作的任务，任务的结果是消息的 ID。 </returns>
    public Task<Guid> SendCardAsync(ICard card, IQuote? quote = null,
        ulong? ephemeralUserId = null, RequestOptions? options = null) =>
        PipeClientHelper.SendCardsAsync(this, [card], quote, ephemeralUserId, options);

    /// <summary>
    ///     发送卡片消息内容到管道。
    /// </summary>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUserId"> 临时消息的接收者的用户 ID。如果设置为指定的用户 ID，则仅该 ID 所关联的用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="options"> 用于配置请求的选项。 </param>
    /// <returns> 返回一个表示异步操作的任务，任务的结果是消息的 ID。 </returns>
    [OverloadResolutionPriority(1)]
    public Task<Guid> SendCardsAsync(IEnumerable<ICard> cards, IQuote? quote = null,
        ulong? ephemeralUserId = null, RequestOptions? options = null) =>
        PipeClientHelper.SendCardsAsync(this, cards, quote, ephemeralUserId, options);

    /// <summary>
    ///     发送卡片消息内容到管道。
    /// </summary>
    /// <param name="parameters"> 要应用到消息管道所使用的消息模板的参数。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUserId"> 临时消息的接收者的用户 ID。如果设置为指定的用户 ID，则仅该 ID 所关联的用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="jsonSerializerOptions"> 序列化模板参数时要使用的序列化选项。 </param>
    /// <param name="options"> 用于配置请求的选项。 </param>
    /// <returns> 返回一个表示异步操作的任务，任务的结果是消息的 ID。 </returns>
    public Task<Guid> SendCardsAsync<T>(T? parameters = default, IQuote? quote = null,
        ulong? ephemeralUserId = null, JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        PipeClientHelper.SendCardsAsync(this, parameters, quote, ephemeralUserId, jsonSerializerOptions, options);

    private static bool ParsePipeUrl(Uri pipeUrl, [NotNullWhen(true)] out string? accessToken)
    {
        NameValueCollection collection = ParseQueryString(pipeUrl);
        accessToken = collection["access_token"];
        return !string.IsNullOrWhiteSpace(accessToken);
    }

    private static NameValueCollection ParseQueryString(Uri url)
    {
        if (string.IsNullOrWhiteSpace(url.Query))
            throw new ArgumentNullException(nameof(url), "The given pipe URL does not contain a query string.");

#if NET462
        NameValueCollection collection = [];
        foreach (string queryItem in url.Query.TrimStart('?').Split('&'))
        {
            int index = queryItem.IndexOf('=');
            if (index == -1)
                collection.Add(queryItem, string.Empty);
            else
                collection.Add(queryItem[..index], queryItem[(index + 1)..]);
        }

        return collection;
#else
        return System.Web.HttpUtility.ParseQueryString(url.Query);
#endif
    }
    /// <inheritdoc />
    public void Dispose()
    {
        ApiClient?.Dispose();
    }
}
