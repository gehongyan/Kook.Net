namespace KaiHeiLa;

public interface IKaiHeiLaClient : IDisposable
{
    Task StartAsync();
    
    Task StopAsync();
}