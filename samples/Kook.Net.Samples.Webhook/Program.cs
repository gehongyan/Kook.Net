using Kook;
using Kook.Webhook;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddKookWebhookClient(config =>
{
    config.TokenType = TokenType.Bot;
    config.Token = default;
    config.VerifyToken = default;
    config.EncryptKey = default;
    config.RouteEndpoint = "kook";
    config.LogLevel = LogSeverity.Debug;
    config.ConfigureKookClient += (serviceProvider, client) =>
    {
        client.Log += message =>
        {
            ILogger<KookWebhookClient> logger = serviceProvider.GetRequiredService<ILogger<KookWebhookClient>>();
            logger.Log(message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Trace,
                _ => throw new ArgumentOutOfRangeException(nameof(message.Severity), message.Severity, null)
            }, message.Exception, "Kook.Webhook: {Message}", message.Message);
            return Task.CompletedTask;
        };
        client.Ready += () =>
        {
            Console.WriteLine("Ready!");
            return Task.CompletedTask;
        };
    };
});

WebApplication app = builder.Build();
app.UseKookEndpoint();
app.Run();
