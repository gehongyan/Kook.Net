using Kook;
using Kook.Net.Samples.Webhook.AspNet;
using Kook.Webhook.AspNet;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddKookAspNetWebhookClient(config =>
{
    config.TokenType = TokenType.Bot;
    config.Token = default;
    config.VerifyToken = default;
    config.EncryptKey = default;
    config.RoutePattern = "kook";
    config.LogLevel = LogSeverity.Debug;
});
builder.Services.AddHostedService<KookClientSubscriptionService>();

WebApplication app = builder.Build();
app.UseKookEndpoint();
await app.RunAsync();
