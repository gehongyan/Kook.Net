using System.Text.Json;
using Kook.API;
using Kook.API.Gateway;
using Kook.Logging;
using Kook.WebSocket;

namespace Kook.Webhook;

/// <summary>
///     Represents a KOOK webhook client.
/// </summary>
public abstract class KookWebhookClient : KookSocketClient
{
    private ConnectionState _connectionState;

    private readonly string? _verifyToken;

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
        _connectionState = ConnectionState.Disconnected;
        _verifyToken = config.VerifyToken;
        WebhookLogger = LogManager.CreateLogger("Webhook");
        ApiClient.WebhookChallenge += OnWebhookChallengeAsync;
    }

    private async Task OnWebhookChallengeAsync(string challenge)
    {
        await WebhookLogger.DebugAsync($"Received Webhook challenge: {challenge}");
        await StartAsyncInternal();
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

    /// <inheritdoc />
    public override ConnectionState ConnectionState => _connectionState;

    internal new KookWebhookApiClient ApiClient => base.ApiClient as KookWebhookApiClient
        ?? throw new InvalidOperationException("The API client is not a Webhook-based client.");

    /// <summary>
    ///     Gets the configuration used by this client.
    /// </summary>
    internal new KookWebhookConfig BaseConfig => base.BaseConfig as KookWebhookConfig
        ?? throw new InvalidOperationException("The base configuration is not a Webhook-based configuration.");

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

    /// <inheritdoc />
    public override async Task StartAsync()
    {
        _connectionState = ConnectionState.Connecting;
        StartMessageQueue();
        if (!BaseConfig.StartupWaitForChallenge)
            await StartAsyncInternal();
    }

    private async Task StartAsyncInternal()
    {
        await FetchRequiredDataAsync();
        _connectionState = ConnectionState.Connected;
    }

    /// <inheritdoc />
    public override async Task StopAsync()
    {
        _connectionState = ConnectionState.Disconnecting;
        await LogoutAsync();
        StopMessageQueue();
        _connectionState = ConnectionState.Disconnected;
    }

    /// <inheritdoc />
    internal override async Task LogoutInternalAsync()
    {
        await ApiClient.GoOfflineAsync(new RequestOptions
        {
            IgnoreState = true
        });
        if (LoginState == LoginState.LoggedOut)
            return;
        LoginState = LoginState.LoggingOut;
        await ApiClient.LogoutAsync().ConfigureAwait(false);
        await OnLogoutAsync().ConfigureAwait(false);
        CurrentUser = null;
        LoginState = LoginState.LoggedOut;
        await _loggedOutEvent.InvokeAsync().ConfigureAwait(false);
    }
}
