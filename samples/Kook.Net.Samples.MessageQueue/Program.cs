using Kook;
using Kook.Net.DependencyInjection.Microsoft;
using Kook.Net.Hosting;
using Kook.Net.Queue.MassTransit;
using Kook.Net.Samples.MessageQueue.MassTransit;
using Kook.Webhook.AspNet;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

string? mode = Environment.GetEnvironmentVariable("KOOK_CLIENT_MODE");
if (mode is "Webhook")
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.Services.AddKook(c =>
    {
        c.UseAspNetWebhookClient(config =>
            {
                config.LogLevel = LogSeverity.Debug;
                config.VerifyToken = string.Empty;
                config.EncryptKey = string.Empty;
                config.LogLevel = LogSeverity.Debug;
                config.TokenType = TokenType.Bot;
                config.Token = string.Empty;
            })
            .UseMassTransitMessageQueue(x =>
            {
                x.UsingInMemory((context, configuration) =>
                {
                    configuration.ConfigureEndpoints(context);
                    // Internal gateway message processing needs to be serialized,
                    // so that the event can be fired with correct cache state.
                    configuration.UseConcurrencyLimit(1);
                });
                x.AddLogging(b => b.AddConsole());
            });
            // To use in-memory message queue, invoke:
            // .UseMessageQueue(SynchronousImmediateMessageQueueProvider.Instance);
    });
    builder.Services.AddHostedService<KookClientSubscriptionService>();
    WebApplication app = builder.Build();
    app.UseKookEndpoint();
    await app.RunAsync();
}
else if (mode is "WebSocket")
{
    HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
    builder.Services.AddKook(c =>
        c.UseSocketClient(config => config.LogLevel = LogSeverity.Debug)
            .UseHostedClient(TokenType.Bot, string.Empty)
            .UseMassTransitMessageQueue(x =>
            {
                x.UsingInMemory((context, configuration) =>
                {
                    configuration.ConfigureEndpoints(context);
                    // Internal gateway message processing needs to be serialized,
                    // so that the event can be fired with correct cache state.
                    configuration.UseConcurrencyLimit(1);
                });
                x.AddLogging(b => b.AddConsole());
            }));
    builder.Services.AddHostedService<KookClientSubscriptionService>();
    IHost app = builder.Build();
    await app.RunAsync();
}
else
{
    throw new InvalidOperationException("Invalid KOOK_CLIENT_MODE");
}
