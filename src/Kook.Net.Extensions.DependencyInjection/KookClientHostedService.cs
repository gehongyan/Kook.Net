using Kook.Rest;
using Microsoft.Extensions.Hosting;

namespace Kook.Net.DependencyInjection;

internal class KookClientHostedService<T> : IHostedService
    where T : IKookClient
{
    private readonly T _kookClient;
    private readonly TokenType? _tokenType;
    private readonly string? _token;
    private readonly bool? _validateToken;

    public KookClientHostedService(T kookClient)
    {
        _kookClient = kookClient;
    }

    public KookClientHostedService(T kookClient, TokenType tokenType, string token, bool validateToken = true)
        : this(kookClient)
    {
        _tokenType = tokenType;
        _token = token;
        _validateToken = validateToken;
    }

    /// <inheritdoc />
    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        if (_kookClient is BaseKookClient baseKookClient
            && _tokenType.HasValue
            && !string.IsNullOrWhiteSpace(_token)
            && _validateToken.HasValue)
            await baseKookClient.LoginAsync(_tokenType.Value, _token, _validateToken.Value);
        await _kookClient.StartAsync();
    }

    /// <inheritdoc />
    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        await _kookClient.StopAsync();
        if (_kookClient is BaseKookClient baseKookClient
            && _tokenType.HasValue
            && !string.IsNullOrWhiteSpace(_token)
            && _validateToken.HasValue)
            await baseKookClient.LogoutAsync();
    }
}
