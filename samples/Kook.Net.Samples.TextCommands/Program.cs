using Kook;
using Kook.Commands;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using TextCommandFramework.Services;

namespace TextCommandFramework;

// This is a minimal example of using Kook.Net's command
// framework - by no means does it show everything the framework
// is capable of.
//
// You can find samples of using the command framework:
// - Here, under the 02_commands_framework sample
// - https://github.com/foxbot/KookBotBase - a bare-bones bot template
// - https://github.com/foxbot/patek - a more feature-filled bot, utilizing more aspects of the library
internal class Program
{
    // There is no need to implement IDisposable like before as we are
    // using dependency injection, which handles calling Dispose for us.
    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        // You should dispose a service provider created using ASP.NET
        // when you are finished using it, at the end of your app's lifetime.
        // If you use another dependency injection framework, you should inspect
        // its documentation for the best way to do this.
        using (ServiceProvider services = ConfigureServices())
        {
            KookSocketClient client = services.GetRequiredService<KookSocketClient>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            // Tokens should be considered secret data and never hard-coded.
            // We can read from the environment variable to avoid hard coding.
            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("KookDebugToken", EnvironmentVariableTarget.User));
            await client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private ServiceProvider ConfigureServices() =>
        new ServiceCollection()
            .AddSingleton(_ => new KookSocketClient(new KookSocketConfig()
            {
                AlwaysDownloadUsers = true, LogLevel = LogSeverity.Debug, AcceptLanguage = "en-US"
            }))
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandlingService>()
            .AddSingleton<HttpClient>()
            .AddSingleton<PictureService>()
            .BuildServiceProvider();
}
