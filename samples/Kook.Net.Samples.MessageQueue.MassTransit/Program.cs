using Kook;
using Kook.Net.Extensions.DependencyInjection;
using Kook.Net.Host;
using Kook.Net.Queue.MassTransit;
using Kook.Net.Samples.MessageQueue.MassTransit;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
builder.Services.AddKook(c =>
{
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
        });
});
builder.Services.AddHostedService<KookClientSubscriptionService>();
IHost app = builder.Build();
await app.RunAsync();
