using System.Net;
using System.Text.Json;
using Kook.API;
using Kook.API.Gateway;
using Kook.API.Rest;
using Kook.Logging;
using Kook.Net;
using Kook.WebSocket;

namespace Kook.Webhook;

/// <summary>
///     Represents a KOOK webhook client.
/// </summary>
public abstract class KookWebhookClient : KookSocketClient
{
    private readonly string? _verifyToken;

    private readonly SemaphoreSlim _stateLock;

    private bool _isDisposed;

    private protected Logger WebhookLogger { get; }

    /// <summary>
    ///     Initializes a new REST/WebSocket-based Kook client with the provided configuration.
    /// </summary>
    /// <param name="config">The configuration to be used with the client.</param>
    protected KookWebhookClient(KookWebhookConfig config)
        : this(config, CreateApiClient(config))
    {
    }

    private protected KookWebhookClient(KookWebhookConfig config, KookWebhookApiClient client)
        : base(config, client)
    {
        _verifyToken = config.VerifyToken;
        WebhookLogger = LogManager.CreateLogger("Webhook");
        ApiClient.WebhookChallenge += OnWebhookChallengeAsync;

        _stateLock = new SemaphoreSlim(1, 1);
        ConnectionManager connectionManager = new(_stateLock, WebhookLogger, config.ConnectionTimeout,
            OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
        connectionManager.Connected += () => TimedInvokeAsync(_connectedEvent, nameof(Connected));
        connectionManager.Disconnected += (ex, _) => TimedInvokeAsync(_disconnectedEvent, nameof(Disconnected), ex);
        Connection = connectionManager;
    }

    internal override ConnectionManager Connection { get; }

    internal new KookWebhookApiClient ApiClient => base.ApiClient as KookWebhookApiClient
        ?? throw new InvalidOperationException("The API client is not a Webhook-based client.");

    /// <summary>
    ///     Gets the configuration used by this client.
    /// </summary>
    internal new KookWebhookConfig BaseConfig => base.BaseConfig as KookWebhookConfig
        ?? throw new InvalidOperationException("The base configuration is not a Webhook-based configuration.");

    private async Task OnConnectingAsync()
    {
        try
        {
            await WebhookLogger.DebugAsync("Connecting ApiClient.");
            SelfOnlineStatusResponse selfOnlineStatus = await ApiClient.GetSelfOnlineStatusAsync();
            if (selfOnlineStatus.OnlineOperatingSystems.Any(x => !"Webhook".Equals(x, StringComparison.OrdinalIgnoreCase)))
            {
                Connection.CriticalError(new InvalidOperationException(
                    $"The client is already online on mode: {string.Join(", ", selfOnlineStatus.OnlineOperatingSystems)}."));
                return;
            }

            if (!selfOnlineStatus.IsOnline)
            {
                await WebhookLogger.WarningAsync("The client is not online, attempting to log in.");
                await ApiClient.GoOnlineAsync();
                await WebhookLogger.InfoAsync("The client has logged in again.");
            }

            _heartbeatTask = RunHeartbeatAsync(Connection.CancellationToken);
            await FetchRequiredDataAsync();
        }
        catch (HttpException ex)
        {
            if (ex.HttpCode == HttpStatusCode.Unauthorized)
                Connection.CriticalError(ex);
            else
                Connection.Error(ex);
        }
        catch
        {
            // ignored
        }
    }

    private async Task OnDisconnectingAsync(Exception ex)
    {
        await WebhookLogger.DebugAsync("Disconnecting ApiClient.");

        //Wait for tasks to complete
        await WebhookLogger.DebugAsync("Waiting for heartbeater").ConfigureAwait(false);
        Task? heartbeatTask = _heartbeatTask;
        if (heartbeatTask != null)
            await heartbeatTask.ConfigureAwait(false);
        _heartbeatTask = null;

        ResetCounter();

        //Raise virtual GUILD_UNAVAILABLEs
        await WebhookLogger.DebugAsync("Raising virtual GuildUnavailables").ConfigureAwait(false);
        foreach (SocketGuild guild in State.Guilds)
            if (guild.IsAvailable)
                await GuildUnavailableAsync(guild).ConfigureAwait(false);
    }

    private async Task OnWebhookChallengeAsync(string challenge)
    {
        await WebhookLogger.DebugAsync($"Received Webhook challenge: {challenge}");
    }

    private static KookWebhookApiClient CreateApiClient(KookWebhookConfig config)
    {
        if (config.EncryptKey is null)
            throw new InvalidOperationException("Encryption key is required.");
        if (config.VerifyToken is null)
            throw new InvalidOperationException("Verify token is required.");
        return new KookWebhookApiClient(config.RestClientProvider, config.WebSocketProvider, config.WebhookProvider,
            config.EncryptKey, config.VerifyToken, KookConfig.UserAgent, config.AcceptLanguage,
            defaultRatelimitCallback: config.DefaultRatelimitCallback);
    }

    internal override void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                try
                {
                    StopAsync().GetAwaiter().GetResult();
                }
                catch (NotSupportedException)
                {
                    // ignored
                }
                ApiClient?.Dispose();
                _stateLock?.Dispose();
            }

            _isDisposed = true;
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc />
    internal override async Task ProcessMessageAsync(GatewaySocketFrameType gatewaySocketFrameType, int? sequence, JsonElement payload)
    {
        if (gatewaySocketFrameType is GatewaySocketFrameType.Event)
        {
            if (!payload.TryGetProperty("verify_token", out JsonElement verifyTokenProperty)
                || verifyTokenProperty.GetString() is not { Length: > 0 } verifyTokenValue)
            {
                await WebhookLogger.WarningAsync(
                        $"Webhook payload is missing the verify token. Payload: {payload}")
                    .ConfigureAwait(false);
            }
            else if (verifyTokenValue != _verifyToken)
            {
                await WebhookLogger.WarningAsync(
                        $"Webhook payload verify token does not match the expected value. Payload: {payload}")
                    .ConfigureAwait(false);
            }
        }

        await base.ProcessMessageAsync(gatewaySocketFrameType, sequence, payload).ConfigureAwait(false);
    }

    private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
    {
        int intervalMillis = BaseConfig.HeartbeatIntervalMilliseconds;
        try
        {
            await WebhookLogger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(intervalMillis, cancellationToken).ConfigureAwait(false);

                try
                {
                    SelfOnlineStatusResponse selfOnlineStatus = await ApiClient.GetSelfOnlineStatusAsync();
                    if (selfOnlineStatus.OnlineOperatingSystems.Any(x => !"Webhook".Equals(x, StringComparison.OrdinalIgnoreCase)))
                    {
                        Connection.CriticalError(new InvalidOperationException(
                            $"The client is already online on mode: {string.Join(", ", selfOnlineStatus.OnlineOperatingSystems)}."));
                    }

                    if (!selfOnlineStatus.IsOnline)
                    {
                        Connection.Reconnect();
                    }
                }
                catch (Exception ex)
                {
                    await WebhookLogger.WarningAsync("Heartbeat Errored", ex).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException)
        {
            await WebhookLogger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await WebhookLogger.ErrorAsync("Heartbeat Errored", ex).ConfigureAwait(false);
        }
    }
}
