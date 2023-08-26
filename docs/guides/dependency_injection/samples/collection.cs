static IServiceProvider CreateServices()
{
    var config = new KookSocketConfig()
    {
        //...
    };

    var collection = new ServiceCollection()
        .AddSingleton(config)
        .AddSingleton<KookSocketClient>();

    return collection.BuildServiceProvider();
}
