using Kook;
using Kook.Net.Samples.Webhook.AspNet;
using Kook.Webhook.AspNet;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddKeyedKookAspNetWebhookClient("Foo", new KookAspNetWebhookConfig
{
    TokenType = TokenType.Bot,
    Token = default,
    VerifyToken = default,
    EncryptKey = default,
    RoutePattern = "kook-foo",
    LogLevel = LogSeverity.Debug
});
builder.Services.AddKeyedKookAspNetWebhookClient("Bar", new KookAspNetWebhookConfig
{
    TokenType = TokenType.Bot,
    Token = default,
    VerifyToken = default,
    EncryptKey = default,
    RoutePattern = "kook-bar",
    LogLevel = LogSeverity.Debug
});
builder.Services.AddHostedService<KookClientSubscriptionService>(provider =>
    new KookClientSubscriptionService(provider, "Foo", "Bar"));

WebApplication app = builder.Build();
app.UseKeyedKookEndpoint("Foo");
app.UseKeyedKookEndpoint("Bar");
await app.RunAsync();

// WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// builder.Services.AddKookAspNetWebhookClient(x =>
// {
//     x.TokenType = TokenType.Bot;
//     x.Token = default;
//     x.VerifyToken = default;
//     x.EncryptKey = default;
//     x.RoutePattern = "kook";
//     x.LogLevel = LogSeverity.Debug;
// });
// builder.Services.AddHostedService<KookClientSubscriptionService>();
//
// WebApplication app = builder.Build();
// app.UseKookEndpoint();
// await app.RunAsync();
