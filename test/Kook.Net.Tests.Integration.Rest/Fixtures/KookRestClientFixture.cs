using Kook.Rest;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Kook;

/// <summary>
///     Test fixture type for integration tests which sets up the client from
///     the token provided in environment variables.
/// </summary>
public class KookRestClientFixture : IAsyncLifetime
{
    public KookRestClient Client { get; private set; } = null!;

    public virtual async ValueTask InitializeAsync()
    {
        string? token = Environment.GetEnvironmentVariable("KOOK_NET_TEST_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("The KOOK_NET_TEST_TOKEN environment variable was not provided.");

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
        await Client.LogoutAsync();
        Client.Dispose();
    }
}
