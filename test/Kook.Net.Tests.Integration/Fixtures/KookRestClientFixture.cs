using Kook.Rest;
using System;

namespace Kook;

/// <summary>
///     Test fixture type for integration tests which sets up the client from
///     the token provided in environment variables.
/// </summary>
public class KookRestClientFixture : IDisposable
{
    public KookRestClient Client { get; private set; }

    public KookRestClientFixture()
    {
        string token = Environment.GetEnvironmentVariable("KOOK_NET_TEST_TOKEN");
        if (string.IsNullOrWhiteSpace(token)) throw new Exception("The KOOK_NET_TEST_TOKEN environment variable was not provided.");

        Client = new KookRestClient(new KookRestConfig() { LogLevel = LogSeverity.Debug, DefaultRetryMode = RetryMode.AlwaysRetry });
        Client.LoginAsync(TokenType.Bot, token).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        Client.LogoutAsync().GetAwaiter().GetResult();
        Client.Dispose();
    }
}
