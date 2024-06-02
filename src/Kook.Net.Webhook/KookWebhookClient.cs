﻿using Kook.API;
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
        _validateToken = config.Value.ValidateToken;
        _webhookLogger = LogManager.CreateLogger("Webhook");
        ApiClient.WebhookChallenge += async challenge =>
            await _webhookLogger.DebugAsync($"Received Webhook challenge: {challenge}");
        config.Value.ConfigureKookClient?.Invoke(serviceProvider, this);
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
}
