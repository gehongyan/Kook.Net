public class UtilizingProvider
{
    private readonly IServiceProvider _provider;
    private readonly AnyService _service;

    // 服务可为 null，因为它只有在提供程序中实际可用时才会填充
    private readonly AnyOtherService? _otherService;

    // 该构造函数仅注入服务提供程序，并使用它来填充其他依赖项
    public UtilizingProvider(IServiceProvider provider)
    {
        _provider = provider;
        _service = provider.GetRequiredService<AnyService>();
        _otherService = provider.GetService<AnyOtherService>();
    }

    // 该构造函数注入服务提供程序和 AnyService，这样无需调用 GetRequiredService页可以确保 AnyService 不为 null
    public UtilizingProvider(IServiceProvider provider, AnyService service)
    {
        _provider = provider;
        _service = service;
        _otherService = provider.GetService<AnyOtherService>();
    }
}
