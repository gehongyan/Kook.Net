using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API;
using Kook.API.Gateway;
using Kook.API.Webhook;
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
    private readonly string? _encryptKey;
    private readonly bool _validateToken;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    ///     Initializes a new REST/WebSocket-based Kook client with the provided configuration.
    /// </summary>
    /// <param name="serviceProvider"> The service provider to be used with the client. </param>
    /// <param name="config">The configuration to be used with the client.</param>
    public KookWebhookClient(IServiceProvider serviceProvider, IOptions<KookWebhookConfig> config)
        : base(config.Value, CreateApiClient(config.Value))
    {
        _connectionState = ConnectionState.Disconnected;
        _tokenType = config.Value.TokenType;
        _token = config.Value.Token;
        _encryptKey = config.Value.EncryptKey;
        _verifyToken = config.Value.VerifyToken;
        _validateToken = config.Value.ValidateToken;
        _webhookLogger = LogManager.CreateLogger("Webhook");
        config.Value.ConfigureKookClient?.Invoke(serviceProvider, this);
    }

    private static KookWebhookApiClient CreateApiClient(KookSocketConfig config) =>
        new(config.RestClientProvider, config.WebSocketProvider, KookConfig.UserAgent,
            config.AcceptLanguage, config.GatewayHost, defaultRatelimitCallback: config.DefaultRatelimitCallback);

    internal string EncryptKey => _encryptKey ?? throw new InvalidOperationException("Encryption key is required.");

    /// <inheritdoc />
    public override ConnectionState ConnectionState => _connectionState;

    /// <inheritdoc />
    public override Task StartAsync() => throw new NotSupportedException("Webhook client does not support starting manually.");

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
            throw new InvalidOperationException("Verify token is required to verify webhook requests.");
        if (_encryptKey is null)
            throw new InvalidOperationException("Encryption key is required to decrypt webhook payloads.");
        await LoginAsync(_tokenType.Value, _token, _validateToken);
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

    internal bool TryParseWebhookChallenge(GatewaySocketFrameType gatewaySocketFrameType, JsonElement payload,
        [NotNullWhen(true)] out string? challenge)
    {
        if (gatewaySocketFrameType != GatewaySocketFrameType.Event
            || !payload.TryGetProperty("type", out JsonElement typeProperty)
            || !typeProperty.TryGetInt32(out int typeValue)
            || typeValue != 255
            || !payload.TryGetProperty("channel_type", out JsonElement channelTypeProperty)
            || channelTypeProperty.GetString() != "WEBHOOK_CHALLENGE")
        {
            challenge = null;
            return false;
        }

        if (!payload.TryGetProperty("verify_token", out JsonElement verifyTokenProperty)
            || verifyTokenProperty.GetString() is not { Length: > 0 } verifyTokenValue)
            throw new InvalidOperationException("Webhook challenge is missing the verify token.");
        if (verifyTokenValue != _verifyToken)
            throw new InvalidOperationException("Webhook challenge verify token does not match.");

        if (!payload.TryGetProperty("challenge", out JsonElement challengeProperty)
            || challengeProperty.GetString() is not { Length: > 0 } challengeValue)
            throw new InvalidOperationException("Webhook challenge is missing the challenge.");

        challenge = challengeValue;
        return true;
    }

    internal new async Task<string?> ProcessMessageAsync(GatewaySocketFrameType gatewaySocketFrameType,
        int? sequence, JsonElement payload)
    {
        if (TryParseWebhookChallenge(gatewaySocketFrameType, payload, out string? challenge))
        {
            await _webhookLogger
                .DebugAsync($"Received webhook challenge: {challenge}")
                .ConfigureAwait(false);
            return JsonSerializer.Serialize(new GatewayChallengeFrame { Challenge = challenge }, SerializerOptions);
        }

        await base.ProcessMessageAsync(gatewaySocketFrameType, sequence, payload);
        return null;
    }
}
