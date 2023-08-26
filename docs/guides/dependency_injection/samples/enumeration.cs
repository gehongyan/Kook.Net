public class ServiceActivator
{
    // 这包含了所有已注册的服务类型为 IService 的服务
    private readonly IEnumerable<IService> _services;

    public ServiceActivator(IEnumerable<IService> services)
    {
        _services = services;
    }

    public async Task ActivateAsync()
    {
        foreach(var service in _services)
        {
            await service.StartAsync();
        }
    }
}
