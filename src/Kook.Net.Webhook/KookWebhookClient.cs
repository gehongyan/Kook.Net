using System.Text.Json;
using Kook.API;
using Kook.API.Gateway;
using Kook.Logging;
using Kook.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Webhook;

/// <summary>
///     Represents a KOOK webhook client.
/// </summary>
public class KookWebhookClient : KookSocketClient, IHostedService
{
    private readonly Logger _webhookLogger;
    private ConnectionState _connectionState;

    private readonly TokenType? _tokenType;
    private readonly string? _token;
    private readonly string? _verifyToken;
    private readonly bool _validateToken;

    /// <summary>
    ///     Initializes a new REST/WebSocket-based Kook client with the provided configuration.
    /// </summary>
    /// <param name="serviceProvider"> The service provider to be used with the client. </param>
    /// <param name="config">The configuration to be used with the client.</param>
    public KookWebhookClient(IServiceProvider serviceProvider, IOptions<KookWebhookConfig> config)
        : this(serviceProvider, config, CreateApiClient(config.Value))
    {
    }

    internal KookWebhookClient(IServiceProvider serviceProvider, IOptions<KookWebhookConfig> config,
        KookWebhookApiClient client)
        : base(config.Value, client)
    {
        _connectionState = ConnectionState.Disconnected;
        _tokenType = config.Value.TokenType;
        _token = config.Value.Token;
        _verifyToken = config.Value.VerifyToken;
        _validateToken = config.Value.ValidateToken;
        _webhookLogger = LogManager.CreateLogger("Webhook");
        ApiClient.WebhookChallenge += OnWebhookChallengeAsync;
        config.Value.ConfigureKookClient?.Invoke(serviceProvider, this);
    }

    private async Task OnWebhookChallengeAsync(string challenge)
    {
        await _webhookLogger.DebugAsync($"Received Webhook challenge: {challenge}");
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
                await _webhookLogger.WarningAsync(
                        $"Webhook payload is missing the verify token. Payload: {payload}")
                    .ConfigureAwait(false);
            }
            else if (verifyTokenValue != _verifyToken)
            {
                await _webhookLogger.WarningAsync(
                        $"Webhook payload verify token does not match the expected value. Payload: {payload}")
                    .ConfigureAwait(false);
            }
        }

        await base.ProcessMessageAsync(gatewaySocketFrameType, sequence, payload).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task StartAsync() =>
        throw new NotSupportedException("Webhook client does not support starting manually.");

    /// <inheritdoc />
    public override Task StopAsync() => Task.CompletedTask;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_tokenType.HasValue)
            throw new InvalidOperationException("Token type is required to log in.");
        if (_token is null)
            throw new InvalidOperationException("Token is required to log in.");
        if (_verifyToken is null)
            throw new InvalidOperationException("Verify token is required to verify the webhook payloads.");
        await LoginAsync(_tokenType.Value, _token, _validateToken);
        if (!BaseConfig.StartupWaitForChallenge)
            await StartAsyncInternal();
    }

    private async Task StartAsyncInternal()
    {
        _connectionState = ConnectionState.Connecting;
        await FetchRequiredDataAsync();
        _connectionState = ConnectionState.Connected;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _connectionState = ConnectionState.Disconnecting;
        await LogoutAsync();
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
