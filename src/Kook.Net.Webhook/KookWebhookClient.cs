using Kook.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kook.Webhook;

/// <summary>
///     Represents a KOOK webhook client.
/// </summary>
public class KookWebhookClient : KookSocketClient, IHostedService
{
    private readonly TokenType _tokenType;
    private readonly string _token;
    private readonly bool _validateToken;

    /// <summary>
    ///     Initializes a new REST/WebSocket-based Kook client with the provided configuration.
    /// </summary>
    /// <param name="serviceProvider"> The service provider to be used with the client. </param>
    /// <param name="config">The configuration to be used with the client.</param>
    public KookWebhookClient(IServiceProvider serviceProvider, IOptions<KookWebhookConfig> config)
        : base(config.Value, CreateApiClient(config.Value))
    {
        _tokenType = config.Value.TokenType;
        _token = config.Value.Token;
        _validateToken = config.Value.ValidateToken;
        Log += config.Value.OnLog(serviceProvider);
    }

    /// <inheritdoc />
    public override Task StartAsync() => throw new NotSupportedException("Webhook client does not support starting.");

    /// <inheritdoc />
    public override Task StopAsync() => throw new NotSupportedException("Webhook client does not support stopping.");

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) => LoginAsync(_tokenType, _token, _validateToken);

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => LogoutAsync();
}
