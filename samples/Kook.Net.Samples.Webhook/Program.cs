using Kook;
using Kook.Webhook;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKookWebhookClient(config =>
{
    config.TokenType = TokenType.Bot;
    config.Token = default;
    config.OnLog = serviceProvider => message =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<KookWebhookClient>>();
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
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.Run();
