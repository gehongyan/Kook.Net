using Kook;
using Kook.WebSocket;

public class Program
{
    private KookSocketClient _client;
    public static Task Main(string[] args) => new Program().MainAsync();
	
    public async Task MainAsync()
    {
        // 如需使用事件中的 Cacheable<IMessage, Guid> 实体，
        // 您可能需要在客户端配置中启用消息缓存。
        var _config = new KookSocketConfig { MessageCacheSize = 100 };
        _client = new KookSocketClient(_config);

        await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("KookToken"));
        await _client.StartAsync();

        _client.MessageUpdated += MessageUpdated;
        _client.Ready += () => 
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        }
		
        await Task.Delay(Timeout.Infinite);
    }

    private async Task MessageUpdated(Cacheable<IMessage, Guid> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // 如果没有启用消息缓存，消息下载方法可能会获得与 `after` 完全相同的实体
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"{message} -> {after}");
    }
}