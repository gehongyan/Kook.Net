using System.Reflection;
using Kook.Commands;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;

public class Initialize
{
    private readonly CommandService _commands;
    private readonly KookSocketClient _client;

    // 如果服务容器内存在 CommandService and KookSocketClient 的实例
    // 则从容器内获取这两个实例，否则需要创建新的实例
    public Initialize(CommandService commands = null, KookSocketClient client = null)
    {
        _commands = commands ?? new CommandService();
        _client = client ?? new KookSocketClient();
    }

    public IServiceProvider BuildServiceProvider() => new ServiceCollection()
        .AddSingleton(_client)
        .AddSingleton(_commands)
        // 可以使用所需类型的实例注入容器
        .AddSingleton(new NotificationService())
        // 也可以通过泛型注入容器
        // 通过泛型注入容器时，ASP.NET 依赖注入服务将会基于构造函数注入所需依赖
        .AddSingleton<DatabaseService>()
        .AddSingleton<CommandHandler>()
        .BuildServiceProvider();
}
public class CommandHandler
{
    private readonly KookSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;

    public CommandHandler(IServiceProvider services, CommandService commands, KookSocketClient client)
    {
        _commands = commands;
        _services = services;
        _client = client;
    }

    public async Task InitializeAsync()
    {
        // 将 ServiceProvider 作为第二个个参数传入 AddModulesAsync
        // 可以将依赖注入到所有可能需要该服务的模块中
        await _commands.AddModulesAsync(
            assembly: Assembly.GetEntryAssembly(), 
            services: _services);
        _client.MessageReceived += HandleCommandAsync;
    }

    public async Task HandleCommandAsync(SocketMessage msg)
    {
        // ...
        // 传入 ExecuteAsync 方法的 ServiceProvider 可用于先决条件
        await _commands.ExecuteAsync(
            context: context, 
            argPos: argPos, 
            services: _services);
        // ...
    }
}