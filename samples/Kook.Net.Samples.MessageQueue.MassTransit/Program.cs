using MassTransit;
using Microsoft.AspNetCore.Builder;

WebApplicationBuilder builder = WebApplication.CreateBuilder();
builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, configuration) =>
    {
        configuration.ConfigureEndpoints(context);
    });
});
WebApplication app = builder.Build();
await app.RunAsync();
