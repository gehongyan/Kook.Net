// KaiHeiLaEventListener.cs

using KaiHeiLa.WebSocket;
using MediatR;
using MediatRSample.Notifications;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace MediatRSample;

public class KaiHeiLaEventListener
{
    private readonly CancellationToken _cancellationToken;

    private readonly KaiHeiLaSocketClient _client;
    private readonly IServiceScopeFactory _serviceScope;

    public KaiHeiLaEventListener(KaiHeiLaSocketClient client, IServiceScopeFactory serviceScope)
    {
        _client = client;
        _serviceScope = serviceScope;
        _cancellationToken = new CancellationTokenSource().Token;
    }

    private IMediator Mediator
    {
        get
        {
            var scope = _serviceScope.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IMediator>();
        }
    }

    public async Task StartAsync()
    {
        _client.MessageReceived += OnMessageReceivedAsync;

        await Task.CompletedTask;
    }

    private Task OnMessageReceivedAsync(SocketMessage arg)
    {
        return Mediator.Publish(new MessageReceivedNotification(arg), _cancellationToken);
    }
}