public class CommandHandler
{
    private readonly KaiHeiLaSocketClient _client;
    private readonly CommandService _commands;

    // 从构造函数中获取 KaiHeiLaSocketClient 与 CommandService 的示例
    public CommandHandler(KaiHeiLaSocketClient client, CommandService commands)
    {
        _commands = commands;
        _client = client;
    }
    
    public async Task InstallCommandsAsync()
    {
        // 将命令服务处理程序订阅至 MessageReceived 事件
        _client.MessageReceived += HandleCommandAsync;

        // 通过反射查找所有命令模块并加载至命令服务
        // 如不使用依赖注入，services 参数传 null
        // 如有需要可参考依赖注入指南
        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
                                        services: null);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // 过滤系统信息
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // 追踪消息前缀结束即文本命令开始的位置
        int argPos = 0;

        // 过滤来自 Bot 的消息，过滤前缀不合命令触发规则的消息
        if (!(message.HasCharPrefix('!', ref argPos) || 
            message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // 基于此消息创建命令上下文
        var context = new SocketCommandContext(_client, message);

        // 执行命令
        await _commands.ExecuteAsync(
            context: context, 
            argPos: argPos,
            services: null);
    }
}