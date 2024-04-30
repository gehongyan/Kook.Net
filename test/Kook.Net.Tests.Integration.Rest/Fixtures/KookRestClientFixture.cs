using Kook.Rest;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Kook;

/// <summary>
///     Test fixture type for integration tests which sets up the client from
///     the token provided in environment variables.
/// </summary>
public class KookRestClientFixture : IDisposable, IAsyncDisposable
{
    public KookRestClient Client { get; private set; }

    public KookRestClientFixture()
    {
        InitializeAsync().GetAwaiter().GetResult();
    }

    [MemberNotNull(nameof(Client))]
    private async Task InitializeAsync()
    {
        string? token = Environment.GetEnvironmentVariable("KOOK_NET_TEST_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("The KOOK_NET_TEST_TOKEN environment variable was not provided.");

        Client = new KookRestClient(new KookRestConfig
        {
            LogLevel = LogSeverity.Debug,
            DefaultRetryMode = RetryMode.AlwaysRetry
        });
        await Client.LoginAsync(TokenType.Bot, token);
    }

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await Client.LogoutAsync();
        Client.Dispose();
    }

    /// <inheritdoc />
    public virtual void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}
