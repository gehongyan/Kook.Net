using KaiHeiLa.API;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

public abstract partial class BaseSocketClient : BaseKaiHeiLaClient
{
    protected readonly KaiHeiLaSocketConfig BaseConfig;
    public abstract int Latency { get; protected set; }
    
    internal new KaiHeiLaSocketApiClient ApiClient => base.ApiClient as KaiHeiLaSocketApiClient;
    
    internal BaseSocketClient(KaiHeiLaSocketConfig config, KaiHeiLaRestApiClient client)
        : base(config, client) => BaseConfig = config;

    
    public abstract Task StartAsync();
    public abstract Task StopAsync();
}