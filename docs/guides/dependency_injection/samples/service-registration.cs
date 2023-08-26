static IServiceProvider CreateServices()
{
    var config = new KookSocketConfig()
    {
        //...
    };

    var servConfig = new CommandServiceConfig()
    {
        //...
    }

    var collection = new ServiceCollection()
        .AddSingleton(config)
        .AddSingleton<KookSocketClient>()
        .AddSingleton(servConfig)
        .AddSingleton<CommandService>();

    return collection.BuildServiceProvider();
}
