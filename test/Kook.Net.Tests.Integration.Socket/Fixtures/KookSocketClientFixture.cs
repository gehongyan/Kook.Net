using System;
using System.Threading.Tasks;
using Kook.WebSocket;
using Xunit;

namespace Kook;

public class KookSocketClientFixture : IAsyncLifetime
{
    private readonly TaskCompletionSource _readyPromise = new();

    public KookSocketClient Client { get; private set; } = null!;

    public virtual async Task InitializeAsync()
    {
        string? token = Environment.GetEnvironmentVariable("KOOK_NET_TEST_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("The KOOK_NET_TEST_TOKEN environment variable was not provided.");

        Client = new KookSocketClient(new KookSocketConfig
        {
            LogLevel = LogSeverity.Debug,
            DefaultRetryMode = RetryMode.AlwaysRetry,
            AlwaysDownloadUsers = true,
            AlwaysDownloadBoostSubscriptions = true,
            AlwaysDownloadVoiceStates = true,
            MessageCacheSize = 100,
            AutoUpdateChannelPositions = true,
            AutoUpdateRolePositions = true
        });
        Client.Ready += ClientOnReady;
        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
        await _readyPromise.Task.WithTimeout();
        Client.Ready -= ClientOnReady;
    }

    private Task ClientOnReady()
    {
        _readyPromise.SetResult();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task DisposeAsync()
    {
        await Client.StopAsync();
        await Client.LogoutAsync();
        Client.Dispose();
    }
}
