using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Kook.WebSocket;

namespace Kook;

public class KookSocketClientFixture : IDisposable, IAsyncDisposable
{
    private readonly TaskCompletionSource _ready = new();

    public KookSocketClient Client { get; private set; }

    public KookSocketClientFixture()
    {
        InitializeAsync().GetAwaiter().GetResult();
    }

    [MemberNotNull(nameof(Client))]
    private async Task InitializeAsync()
    {
        string? token = Environment.GetEnvironmentVariable("KOOK_NET_TEST_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("The KOOK_NET_TEST_TOKEN environment variable was not provided.");

        Client = new KookSocketClient(new KookSocketConfig
        {
            LogLevel = LogSeverity.Debug,
            DefaultRetryMode = RetryMode.AlwaysRetry,
            AlwaysDownloadUsers = false,
            AlwaysDownloadBoostSubscriptions = false,
            AlwaysDownloadVoiceStates = false
        });
        Client.Ready += ClientOnReady;
        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
        await _ready.Task;
        Client.Ready -= ClientOnReady;
    }

    private Task ClientOnReady()
    {
        _ready.SetResult();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await Client.StopAsync();
        await Client.LogoutAsync();
        Client.Dispose();
    }

    /// <inheritdoc />
    public virtual void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}
