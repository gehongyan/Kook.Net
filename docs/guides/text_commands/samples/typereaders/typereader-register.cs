public class CommandHandler
{
    private readonly CommandService _commands;
    private readonly KookSocketClient _client;
    private readonly IServiceProvider _services;

    public CommandHandler(CommandService commands, KookSocketClient client, IServiceProvider services)
    {
        _commands = commands;
        _client = client;
        _services = services;
    }

    public async Task SetupAsync()
    {
        _client.MessageReceived += CommandHandleAsync;

        // 添加 BooleanTypeReader 来解析 `bool` 类型的参数
        _commands.AddTypeReader(typeof(bool), new BooleanTypeReader());

        // 注册模块
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    public async Task CommandHandleAsync(SocketMessage msg)
    {
        // ...
    }
}