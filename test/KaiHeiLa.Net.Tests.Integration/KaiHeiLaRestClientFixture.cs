using KaiHeiLa.Rest;
using System;
using Xunit;

namespace KaiHeiLa
{
    /// <summary>
    ///     Test fixture type for integration tests which sets up the client from
    ///     the token provided in environment variables.
    /// </summary>
    public class KaiHeiLaRestClientFixture : IDisposable
    {
        public KaiHeiLaRestClient Client { get; private set; }

        public KaiHeiLaRestClientFixture()
        {
            var token = Environment.GetEnvironmentVariable("KAIHEILA_NET_TEST_TOKEN", EnvironmentVariableTarget.User);
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("The KAIHEILA_NET_TEST_TOKEN environment variable was not provided.");
            Client = new KaiHeiLaRestClient(new KaiHeiLaRestConfig()
            {
                LogLevel = LogSeverity.Debug,
                DefaultRetryMode = RetryMode.AlwaysRetry
            });
            Client.LoginAsync(TokenType.Bot, token).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            Client.LogoutAsync().GetAwaiter().GetResult();
            Client.Dispose();
        }
    }
}
